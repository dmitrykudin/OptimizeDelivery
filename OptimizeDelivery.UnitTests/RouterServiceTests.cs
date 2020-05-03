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
        [TestCase(@"D:\Maps.pbf\Pbf\saint-petersburg-extended.osm.pbf",
            @"D:\Maps.pbf\RouterDb\saint-petersburg-extended.routerdb")]
        public void CreateRouterDbFileTest(string filePath, string savePath)
        {
            ItineroRouter.CreateRouterDbFile(filePath, savePath);
        }

        [Test]
        [TestCase(5)]
        public void GetTimeMatrixTest(int coordinatesAmount)
        {
            var districtService = new DistrictService();
            var allDistricts = districtService.GetAllDistricts();
            var random = new Random();
            var randomDistrictId = allDistricts[random.Next(allDistricts.Length)].Id;

            var routerDb = ItineroRouter.GetRouterDb(randomDistrictId);
            var router = ItineroRouter.GetRouter(randomDistrictId);

            var coordinates = RandHelper.CoordinateSetFrom(routerDb, coordinatesAmount);
            for (var i = 0; i < coordinatesAmount; i++) Console.WriteLine(coordinates[i]);
            var resultPoints = coordinates.Select(x => router.Resolve(Vehicle.Car.Fastest(), x)).ToArray();

            var weightTimeMatrix = ItineroRouter.GetWeightTimeMatrix(resultPoints);
            Console.WriteLine("\nUsing GetWeightTimeMatrix");
            weightTimeMatrix.OutputMatrix();

            var timeMatrix = ItineroRouter.GetTimeMatrix(resultPoints);
            Console.WriteLine("\nUsing GetTimeMatrix");
            timeMatrix.OutputMatrix();
        }

        [Test]
        public void OptimizeRoutesTest()
        {
            var optimizationService = new OptimizationService();
            optimizationService.BuildOptimalRoutes(useDistricts: false);
        }

        [Test]
        public void ResolveShouldReturnValidCoordinateTest()
        {
            var routerDb = ItineroRouter.GetRouterDb();
            var router = ItineroRouter.GetRouter();
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
    }
}