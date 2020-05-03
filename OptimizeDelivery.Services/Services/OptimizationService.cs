using System;
using System.Collections.Generic;
using System.Linq;
using Common.Abstractions.Services;
using Common.Helpers;
using Common.Models.BusinessModels;
using Common.Models.FilterModels;
using Common.Models.ServiceModels;
using Google.OrTools.ConstraintSolver;
using Itinero;
using Itinero.Osm.Vehicles;
using Route = Common.Models.BusinessModels.Route;
using ItineroRoute = Itinero.Route;

namespace OptimizeDelivery.Services.Services
{
    public class OptimizationService
    {
        private IRouteService RouteService { get; set; }

        public IParcelService ParcelService { get; set; }

        public OptimizationService()
        {
            RouteService = new RouteService();
            ParcelService = new ParcelService();
        }
        
        public void BuildOptimalRoutes(bool useDistricts)
        {
            var parcelService = new ParcelService();
            var depotService = new DepotService();

            var parcelsByDepot = parcelService
                .GetParcels(new ParcelFilter
                {
                    DeliveryDate = new FilterValue<DateTime?>(DateTime.Today),
                    RouteId = new FilterValue<int?>(null),
                })
                .GroupBy(x => x.DepotId);

            foreach (var depotParcels in parcelsByDepot)
            {
                var depot = depotService.GetDepot(depotParcels.Key);
                var parcels = depotParcels.ToArray();

                if (useDistricts)
                {
                    var parcelsByDistrict = parcels.GroupBy(x => x.DistrictId);
                    foreach (var districtParcels in parcelsByDistrict)
                    {
                        var optimalRoutePlans = GetOptimalRoutePlans(districtParcels.ToArray(), depot);
                        BuildAndSaveRoutes(depot, parcels, optimalRoutePlans);
                    }
                }
                else
                {
                    var optimalRoutePlans = GetOptimalRoutePlans(parcels, depot);
                    BuildAndSaveRoutes(depot, parcels, optimalRoutePlans);
                }
            }

            // TODO
            // - Add Couriers here
        }

        private OptimalRoutePlan[] GetOptimalRoutePlans(Parcel[] parcels, Depot depot)
        {
            var router = ItineroRouter.GetRouter();

            var coordinates = parcels.Select(x => x.RoutableCoordinate).ToArray();
            var timeWindows = parcels
                .Select(x => x.DeliveryTimeWindow.GetWindow())
                .ToArray();

            var coordinatesWithDepot = new[] {depot.RoutableCoordinate}.Append(coordinates);
            var timeWindowsWithDepot = new[] {depot.WorkingTimeWindow.GetWindow()}.Append(timeWindows);

            var timeMatrix = ItineroRouter.GetWeightTimeMatrix(coordinatesWithDepot
                .Select(x => router.Resolve(Vehicle.Car.Fastest(), x, 200F))
                .ToArray());

            timeMatrix.OutputMatrix();
            return GetOptimalRoutePlans(timeMatrix.ForOptimization(), timeWindowsWithDepot.CreateRectangularArray(), 10, 0);
        }

        private void BuildAndSaveRoutes(Depot depot, Parcel[] parcels, OptimalRoutePlan[] routePlans)
        {
            var router = ItineroRouter.GetRouter();

            foreach (var routePlan in routePlans)
            {
                var destinations = routePlan.OrderedDestinations;
                var routerPoints = new RouterPoint[destinations.Length];
                var currentRouteParcels = new Parcel[destinations.Length - 2];

                // Depot is the first and the last point
                routerPoints[0] = routerPoints[destinations.Length - 1] = router.Resolve(Vehicle.Car.Fastest(), depot.RoutableCoordinate);
                for (var i = 1; i < destinations.Length - 1; i++)
                {
                    var currentParcel = parcels[destinations[i].DestinationId - 1];
                    currentParcel.RoutePosition = i;
                    currentRouteParcels[i - 1] = currentParcel;
                    routerPoints[i] = router.Resolve(Vehicle.Car.Fastest(), currentParcel.RoutableCoordinate);
                }

                var route = RouteService.CreateRoute(new Route
                {
                    TotalTime = Convert.ToInt32(routePlan.TotalTime),
                    CreationDate = DateTime.Now,
                    RouteJsonDetails = router
                        .Calculate(Vehicle.Car.Fastest(), routerPoints)
                        .ToGeoJson(),
                });
                
                ParcelService.UpdateParcelsRoute(route.Id, currentRouteParcels);
            }
        }

        #region OrTools Route Optimization

        private OptimalRoutePlan[] GetOptimalRoutePlans(long[,] timeMatrix, long[,] timeWindows, int vehicleNumber,
            int depotId)
        {
            // Create Routing Index Manager
            var manager = new RoutingIndexManager(timeMatrix.GetLength(0), vehicleNumber, depotId);

            // Create Routing Model.
            var routing = new RoutingModel(manager);

            // Create and register a transit callback. Returns weight or arc between two nodes.
            var transitCallbackIndex = routing.RegisterTransitCallback(
                (fromIndex, toIndex) =>
                {
                    // Convert from routing variable Index to distance matrix NodeIndex.
                    var fromNode = manager.IndexToNode(fromIndex);
                    var toNode = manager.IndexToNode(toIndex);
                    return timeMatrix[fromNode, toNode];
                }
            );

            // Define cost of each arc.
            routing.SetArcCostEvaluatorOfAllVehicles(transitCallbackIndex);

            // Add Distance constraint.
            routing.AddDimension(
                transitCallbackIndex, // transit callback
                60 * 60, // allow waiting time
                1440 * 60, // vehicle maximum capacities
                false, // start cumul to zero
                "Time");
            var timeDimension = routing.GetMutableDimension("Time");
            // Add time window constraints for each location except depot.
            for (var i = 1; i < timeWindows.GetLength(0); ++i)
            {
                var index = manager.NodeToIndex(i);
                timeDimension.CumulVar(index).SetRange(
                    timeWindows[i, 0],
                    timeWindows[i, 1]);
            }

            // Add time window constraints for each vehicle start node.
            for (var i = 0; i < vehicleNumber; ++i)
            {
                var index = routing.Start(i);
                timeDimension.CumulVar(index).SetRange(
                    timeWindows[0, 0],
                    timeWindows[0, 1]);
            }

            // Instantiate route start and end times to produce feasible times.
            for (var i = 0; i < vehicleNumber; ++i)
            {
                routing.AddVariableMinimizedByFinalizer(
                    timeDimension.CumulVar(routing.Start(i)));
                routing.AddVariableMinimizedByFinalizer(
                    timeDimension.CumulVar(routing.End(i)));
            }

            // Setting first solution heuristic.
            var searchParameters =
                operations_research_constraint_solver.DefaultRoutingSearchParameters();
            searchParameters.FirstSolutionStrategy =
                FirstSolutionStrategy.Types.Value.Automatic;

            // Solve the problem.
            var solution = routing.SolveWithParameters(searchParameters);

            // Print solution on console.
            PrintSolution(vehicleNumber, routing, manager, solution);

            return BuildOptimizedRoutes(vehicleNumber, routing, manager, solution);
        }

        private static void PrintSolution(int vehicleNumber, in RoutingModel routing, in RoutingIndexManager manager,
            in Assignment solution)
        {
            var timeDimension = routing.GetMutableDimension("Time");
            // Inspect solution.
            long totalTime = 0;
            for (var i = 0; i < vehicleNumber; ++i)
            {
                Console.WriteLine("Route for Vehicle {0}:", i);
                var index = routing.Start(i);
                while (routing.IsEnd(index) == false)
                {
                    var timeVar = timeDimension.CumulVar(index);
                    Console.Write("{0} Time({1},{2}) -> ",
                        manager.IndexToNode(index),
                        solution.Min(timeVar),
                        solution.Max(timeVar));
                    index = solution.Value(routing.NextVar(index));
                }

                var endTimeVar = timeDimension.CumulVar(index);
                Console.WriteLine("{0} Time({1},{2})",
                    manager.IndexToNode(index),
                    solution.Min(endTimeVar),
                    solution.Max(endTimeVar));
                Console.WriteLine("Time of the route: {0} seconds", solution.Min(endTimeVar));
                totalTime += solution.Min(endTimeVar);
            }

            Console.WriteLine("Total time of all routes: {0} seconds", totalTime);
        }

        private static OptimalRoutePlan[] BuildOptimizedRoutes(int vehicleNumber, RoutingModel routingModel, RoutingIndexManager manager,
            Assignment solution)
        {
            void AddNode(RoutingDimension dimension, long index, ICollection<RoutePlanDestination> destinations)
            {
                var timeVar = dimension.CumulVar(index);
                destinations.Add(new RoutePlanDestination
                {
                    DestinationId = manager.IndexToNode(index),
                    ArrivalTimeFrom = solution.Min(timeVar),
                    ArrivalTimeTo = solution.Max(timeVar),
                });
            }

            var timeDimension = routingModel.GetMutableDimension("Time");
            var optimizedRoutes = new OptimalRoutePlan[vehicleNumber];
            for (var i = 0; i < vehicleNumber; i++)
            {
                var index = routingModel.Start(i);
                var orderedDestinations = new List<RoutePlanDestination>();
                while (routingModel.IsEnd(index) == false)
                {
                    AddNode(timeDimension, index, orderedDestinations);
                    index = solution.Value(routingModel.NextVar(index));
                }
                
                AddNode(timeDimension, index, orderedDestinations);
                optimizedRoutes[i] = new OptimalRoutePlan
                {
                    TotalTime = orderedDestinations.Last().ArrivalTimeFrom,
                    OrderedDestinations = orderedDestinations.ToArray(),
                };
            }

            return optimizedRoutes;
        }

        #endregion
    }
}