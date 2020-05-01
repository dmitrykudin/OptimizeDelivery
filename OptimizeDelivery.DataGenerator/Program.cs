using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Helpers;
using Common.Models.BusinessModels;
using Common.Models.FilterModels;
using OptimizeDelivery.Services.Services;

namespace OptimizeDelivery.DataGenerator
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            RunIdealCoordinateGeneration();
        }

        private static void RunIdealCoordinateGeneration()
        {
            var idealCoordinateService = new IdealCoordinateService();
            using (var source = new CancellationTokenSource())
            {
                var token = source.Token;
                var task = idealCoordinateService.Run(token);

                Console.WriteLine("Press any key to finish the operation...");
                Console.ReadKey();
                source.Cancel();
                task.Wait();
            }

            Console.WriteLine("Operation finished.");
        }
        
        private static void OptimizeDelivery()
        {
            var parcelService = new ParcelService();
            var clusterizationService = new ClusterizationService();
            var districtService = new DistrictService();
            var courierService = new CourierService();

            Sandbox.CreateTestData(100);
            var parcelsForToday = parcelService.GetParcels(new ParcelFilter { DeliveryDate = DateTime.Today });
            var districts = districtService.GetAllDistricts();
            var (parcelsPerDistrict, nonClusteredParcels) = clusterizationService.ClusterParcelsByDistricts(districts, parcelsForToday);

            PrintDistrictsLocations(parcelsPerDistrict);
            
            Console.WriteLine("Non-clustered parcels: ");

            foreach (var nonClusteredParcel in nonClusteredParcels)
            {
                Console.WriteLine(nonClusteredParcel.OriginalLocation.ToStringNoWhitespace());
            }

            /*foreach (var districtWithParcels in parcelsPerDistrict)
            {
                
            }*/
        }
        
        private static void FillDistrictsTable()
        {
            Sandbox.FillDistrictsTable();
        }

        private static void PrintDistrictsLocations(Dictionary<District, List<Parcel>> districtsWithParcels)
        {
            foreach (var district in districtsWithParcels)
            {
                Console.WriteLine("Locations for district: " + district.Key.Name);
                foreach (var parcel in district.Value)
                {
                    Console.WriteLine(parcel.OriginalLocation.ToStringNoWhitespace());
                }
                Console.WriteLine("\n\n");
            }
        }
        
        private static void OldDeliveryOptimizationTest()
        {
            Sandbox.CreateTestData(100);

            var optimizeDeliveryService = new OptimizeDeliveryService();
            var parcelService = new ParcelService();
            var clusterizationService = new ClusterizationService();
            var testDataService = new TestDataService();

            Console.WriteLine("Test data created.");

            var depot = testDataService.GetLastDepot();

            Console.WriteLine("Using depot with Id: " + depot.Id);

            var parcelsForToday = parcelService.GetParcels(new ParcelFilter { DeliveryDate = DateTime.Today });

            Console.WriteLine("Loaded parcels for today: " + parcelsForToday.Length);

            var deliveryClustersBalanced = clusterizationService.ClusterParcelLocationsBalanced(parcelsForToday);

            Console.WriteLine("Made clusters, count: " + deliveryClustersBalanced.Length);

            var mapRoutes = optimizeDeliveryService.MakeRoutesForClusters(deliveryClustersBalanced, depot);

            Console.WriteLine("Created routes, count: " + mapRoutes.Length);

            Console.WriteLine("Finished");
        }
    }
}