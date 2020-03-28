using System.Diagnostics;
using System.IO;
using Itinero;
using Itinero.Osm.Vehicles;

namespace OptimizeDelivery.ItineroSandbox
{
    internal class Program
    {
        private static RouterDb RouterDb { get; set; }

        public static void Main(string[] args)
        {
            using (var stream = new FileInfo(@"D:/Maps.pbf/saint-petersburg.routerdb").OpenRead())
            {
                RouterDb = RouterDb.Deserialize(stream);
            }

            var router = new Router(RouterDb);

            var route = router.Calculate(Vehicle.Car.Fastest(), 59.8279f, 30.5549f, 60.0295f, 30.2363f);
            var geoJson = route.ToGeoJson();

            Debug.Write(geoJson);
        }
    }
}