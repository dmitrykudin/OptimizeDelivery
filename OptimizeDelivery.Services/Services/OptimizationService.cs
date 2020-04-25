using System;
using Google.OrTools.ConstraintSolver;

namespace OptimizeDelivery.Services.Services
{
    public class OptimizationService
    {
        public void OptimizeRoutesWithTimeMatrix(long[,] timeMatrix, long[,] timeWindows, int vehicleNumber,
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
                120, // allow waiting time
                1200, // vehicle maximum capacities
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
                Console.WriteLine("Time of the route: {0}min", solution.Min(endTimeVar));
                totalTime += solution.Min(endTimeVar);
            }

            Console.WriteLine("Total time of all routes: {0}min", totalTime);
        }
    }
}