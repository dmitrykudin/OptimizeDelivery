using System;
using System.Collections.Generic;
using System.Linq;
using Common.Helpers;
using Common.Models.BusinessModels;
using OptimizeDelivery.Services.Services;

namespace OptimizeDelivery.DataGenerator
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var optimizeDeliveryService = new OptimizeDeliveryService();
            var clusterizationService = new ClusterizationService();
            var districtService = new DistrictService();
            var courierService = new CourierService();

            Sandbox.CreateTestData(100);
            var parcelsForToday = optimizeDeliveryService.GetParcelsForToday().ToArray();
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
            var clusterizationService = new ClusterizationService();
            var testDataService = new TestDataService();

            Console.WriteLine("Test data created.");

            var depot = testDataService.GetLastDepot();

            Console.WriteLine("Using depot with Id: " + depot.Id);

            var parcelsForToday = optimizeDeliveryService.GetParcelsForToday().ToArray();

            Console.WriteLine("Loaded parcels for today: " + parcelsForToday.Length);

            var deliveryClustersBalanced = clusterizationService.ClusterParcelLocationsBalanced(parcelsForToday);

            Console.WriteLine("Made clusters, count: " + deliveryClustersBalanced.Length);

            var mapRoutes = optimizeDeliveryService.MakeRoutesForClusters(deliveryClustersBalanced, depot);

            Console.WriteLine("Created routes, count: " + mapRoutes.Length);

            Console.WriteLine("Finished");
        }
    }
}