namespace OptimizeDelivery.DataGenerator
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Sandbox.FillDistrictsTable();
/*
            Sandbox.CreateTestData();

            var optimizeDeliveryService = new OptimizeDeliveryService();
            var clusterizationService = new ClusterizationService();
            var testDataService = new TestDataService();

            Console.WriteLine("Test data created.");

            var depot = testDataService.GetLastDepot();

            Console.WriteLine("Using depot with Id: " + depot.Id);

            var parcelsForToday = optimizeDeliveryService.GetParcelsForToday();

            Console.WriteLine("Loaded parcels for today: " + parcelsForToday.Count());

            var deliveryClustersBalanced = clusterizationService.ClusterParcelLocationsBalanced(parcelsForToday);

            Console.WriteLine("Made clusters, count: " + deliveryClustersBalanced.Length);

            var mapRoutes = optimizeDeliveryService.MakeRoutesForClusters(deliveryClustersBalanced, depot);

            Console.WriteLine("Created routes, count: " + mapRoutes.Length);

            Console.WriteLine("Finished");
*/
        }
    }
}