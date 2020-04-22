using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common.Helpers;
using Common.Models.BusinessModels;
using Itinero;
using Itinero.Algorithms.Weights;
using Itinero.IO.Osm;
using Itinero.Osm.Vehicles;

namespace OptimizeDelivery.Services.Services
{
    public static class RouterService
    {
        private static Router Router { get; set; }
        
        private static RouterDb RouterDb { get; set; }

        public static void CreateRouterDbFile(string filePath, string savePath)
        {
            var routerDb = new RouterDb();
            using (var stream = new FileInfo(filePath).OpenRead())
            {
                routerDb.LoadOsmData(stream, Vehicle.Car);
            }

            using (var stream = new FileInfo(savePath).Open(FileMode.Create))
            {
                routerDb.Serialize(stream);
            }
        }
        
        public static RouterDb GetRouterDb()
        {
            if (RouterDb == null)
            {
                using (var stream = new FileInfo(@"D:/Maps.pbf/RouterDb/spb-central-district.routerdb").OpenRead())
                {
                    RouterDb = RouterDb.Deserialize(stream);
                }
                RouterDb.AddContracted(Vehicle.Car.Fastest());
                // RouterDb.AddContracted(Vehicle.Car.Shortest());
            }

            return RouterDb;
        }

        public static Router GetRouter()
        {
            if (Router == null)
            {
                Router = new Router(GetRouterDb());
            }

            return Router;
        }

        public static float[][] GetTimeMatrix(Parcel[] parcels)
        {
            var routerPoints = parcels.Select(x => 
                GetRouter().Resolve(Vehicle.Car.Fastest(), x.Location.ToItineroCoordinate(), 100F));
            return GetTimeMatrix(routerPoints.ToArray());
        }

        public static float[][] GetTimeMatrix(RouterPoint[] coordinates)
        {
            var router = GetRouter();
            ISet<int> invalidCoordinates = new HashSet<int>();
            var result = router.CalculateWeight(Vehicle.Car.Fastest(), coordinates,  invalidCoordinates);
            return result;
        }
        
        public static long[,] GetWeightTimeMatrix(RouterPoint[] coordinates)
        {
            var router = GetRouter();
            ISet<int> invalidCoordinates = new HashSet<int>();
            var result = router.CalculateWeight(
                Vehicle.Car.Fastest(), 
                router.GetAugmentedWeightHandler(Vehicle.Car.Fastest()), 
                coordinates, 
                invalidCoordinates).ToTimeMatrix();
            return result;
        }

        private static long[,] ToTimeMatrix(this Weight[][] weightTimeMatrix)
        {
            var height = weightTimeMatrix.Length;
            var width = weightTimeMatrix[0].Length;
            var timeMatrix = new long[width, height];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    timeMatrix[i, j] = Convert.ToInt64(weightTimeMatrix[i][j].Time);
                }
            }

            return timeMatrix;
        }
    }
}