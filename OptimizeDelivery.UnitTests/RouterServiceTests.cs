using System;
using System.Linq;
using Common.Helpers;
using Itinero;
using Itinero.LocalGeo;
using Itinero.Osm.Vehicles;
using NUnit.Framework;
using OptimizeDelivery.Services.Services;

namespace OptimizeDelivery.UnitTests
{
    public class RouterServiceTests
    {
        [Test]
        [Ignore("Generates .routerdb files from .pbf. Run only manually.")]
        [TestCase(@"D:\Maps.pbf\Pbf\saint-petersburg-extended.osm.pbf", @"D:\Maps.pbf\RouterDb\saint-petersburg-extended.routerdb")]
        public void CreateRouterDbFileTest(string filePath, string savePath)
        {
            RouterService.CreateRouterDbFile(filePath, savePath);
        }

        [Test]
        [TestCase(5)]
        public void GetTimeMatrixTest(int coordinatesAmount)
        {
            var districtService = new DistrictService();
            var allDistricts = districtService.GetAllDistricts();
            var random = new Random();
            var randomDistrictId = allDistricts[random.Next(allDistricts.Length)].Id;

            var routerDb = RouterService.GetRouterDb(randomDistrictId);
            var router = RouterService.GetRouter(randomDistrictId);

            var coordinates = RandHelper.CoordinateSetFrom(routerDb, coordinatesAmount);
            for (var i = 0; i < coordinatesAmount; i++)
            {
                Console.WriteLine(coordinates[i]);
            }
            var resultPoints = coordinates.Select(x => router.Resolve(Vehicle.Car.Fastest(), x)).ToArray();

            var weightTimeMatrix = RouterService.GetWeightTimeMatrix(resultPoints);
            Console.WriteLine("\nUsing GetWeightTimeMatrix");
            OutputMatrix(weightTimeMatrix);

            var timeMatrix = RouterService.GetTimeMatrix(resultPoints);
            Console.WriteLine("\nUsing GetTimeMatrix");
            OutputMatrix(timeMatrix);
        }

        [Test]
        public void OptimizeRoutesTest()
        {
            var router = RouterService.GetRouter();
            var optimizationService = new OptimizationService();
            var coordinates = new[]
            {
                new Coordinate(59.9202541272f, 30.3386695619f),
                new Coordinate(59.9190939589f, 30.3402878697f),
                new Coordinate(59.9183610857f, 30.3409329032f),
                new Coordinate(59.9295678254f, 30.3201181123f),
                new Coordinate(59.9334973694f, 30.3139640327f),
                new Coordinate(59.9380210072f, 30.3126046687f)
            };

            var timeWindows = new long[,]
            {
                {0, 360},
                {0, 600},
                {420, 720},
                {240, 660},
                {780, 1200},
                {360, 420}
            };

            var timeMatrix = RouterService.GetWeightTimeMatrix(coordinates
                .Select(x => router.Resolve(Vehicle.Car.Fastest(), x, 200F))
                .ToArray());

            OutputMatrix(timeMatrix);
            var optimizedRoutes = optimizationService.OptimizeRoutesWithTimeMatrix(timeMatrix.ForOptimization(), timeWindows, 2, 0);
            
            // TODO
            // - Add Parcels and Couriers here
            // - optimizedRoutes -> Route with Parcels order
            // - Calculate route via Itinero and store it
        }

        [Test]
        public void ResolveShouldReturnValidCoordinateTest()
        {
            var routerDb = RouterService.GetRouterDb();
            var router = RouterService.GetRouter();
            var coordinates = new[]
            {
                new Coordinate(59.9223517599f, 30.3359459534f),
                new Coordinate(59.9227905443f, 30.3356373828f),
                new Coordinate(59.9255812284f, 30.3314824308f),
                new Coordinate(59.9148432376f, 30.3526438049f),
                new Coordinate(59.9238480014f, 30.3339749262f),
                new Coordinate(59.9240496192f, 30.3340876673f),
                new Coordinate(59.9138591227f, 30.3631853831f),
                new Coordinate(59.9208568929f, 30.3378848622f),
                new Coordinate(59.9235119307f, 30.3351878157f),
                new Coordinate(59.9398907331f, 30.3098232578f)
            };

            foreach (var coordinate in coordinates)
            {
                var resolvedCoordinate = router
                    .Resolve(Vehicle.Car.Fastest(), coordinate)
                    .LocationOnNetwork(routerDb);
                Console.Write("Coordinate " + coordinate + " resolved to " + resolvedCoordinate + "\n");
            }
        }

        private void OutputMatrix(float[][] matrix)
        {
            var rowLength = matrix.GetLength(0);
            var colLength = matrix[0].GetLength(0);
            for (var i = 0; i < rowLength; i++)
            {
                for (var j = 0; j < colLength; j++) Console.Write($"{matrix[i][j],12} ");
                Console.Write(Environment.NewLine + Environment.NewLine);
            }
        }
    }
}