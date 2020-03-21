using System.IO;
using Itinero;
using Itinero.IO.Osm;
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
        }
    }
}