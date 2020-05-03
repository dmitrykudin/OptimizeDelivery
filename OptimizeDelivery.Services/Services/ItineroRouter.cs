using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.IO;
using System.Linq;
using Common.Abstractions.Services;
using Common.Constants;
using Common.Helpers;
using Common.Models.BusinessModels;
using Itinero;
using Itinero.IO.Osm;
using Itinero.LocalGeo;
using Itinero.Profiles;
using Vehicle = Itinero.Osm.Vehicles.Vehicle;

namespace OptimizeDelivery.Services.Services
{
    public static class ItineroRouter
    {
        public static readonly Profile DefaultProfile = Vehicle.Car.Fastest();

        private static Router GlobalRouter { get; set; }

        private static RouterDb GlobalRouterDb { get; set; }

        private static Dictionary<int, RouterDb> RouterDbCache { get; set; }
        
        private static Dictionary<int, Router> RouterCache { get; set; }
        
        private static IDistrictService DistrictService => new DistrictService();

        #region Router

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
            if (GlobalRouterDb == null)
            {
                using (var stream = new FileInfo(Const.GlobalRouterDbFilePath).OpenRead())
                {
                    GlobalRouterDb = RouterDb.Deserialize(stream);
                }

                GlobalRouterDb.AddContracted(DefaultProfile);
            }

            return GlobalRouterDb;
        }

        public static RouterDb GetRouterDb(int? districtId)
        {
            if (!districtId.HasValue)
            {
                return GetRouterDb();
            }
            
            RouterDbCache ??= new Dictionary<int, RouterDb>();

            if (RouterDbCache.TryGetValue(districtId.Value, out var existingRouterDb))
            {
                return existingRouterDb;
            }

            var district = DistrictService.GetDistrict(districtId.Value);
            RouterDb newRouterDb;
            using (var stream = new FileInfo(district.RouterDbFilePath).OpenRead())
            {
                newRouterDb = RouterDb.Deserialize(stream);
            }

            newRouterDb.AddContracted(DefaultProfile);
            RouterDbCache.Add(districtId.Value, newRouterDb);

            return newRouterDb;
        }

        public static Router GetRouter()
        {
            if (GlobalRouter == null) GlobalRouter = new Router(GetRouterDb());

            return GlobalRouter;
        }

        public static Router GetRouter(int? districtId)
        {
            if (!districtId.HasValue)
            {
                return GetRouter();
            }
            
            RouterCache ??= new Dictionary<int, Router>();

            if (RouterCache.TryGetValue(districtId.Value, out var existingRouter))
            {
                return existingRouter;
            }

            var router = new Router(GetRouterDb(districtId));
            RouterCache.Add(districtId.Value, router);

            return router;
        }

        #endregion

        #region TimeMatrix

        public static float[][] GetTimeMatrix(Parcel[] parcels)
        {
            var routerPoints = parcels.Select(x =>
                GetRouter().Resolve(DefaultProfile, x.OriginalLocation.ToItineroCoordinate(), 100F));
            return GetTimeMatrix(routerPoints.ToArray());
        }

        public static float[][] GetTimeMatrix(RouterPoint[] coordinates)
        {
            var router = GetRouter();
            ISet<int> invalidCoordinates = new HashSet<int>();
            var result = router.CalculateWeight(DefaultProfile, coordinates, invalidCoordinates);
            return result;
        }

        public static float[][] GetWeightTimeMatrix(RouterPoint[] coordinates)
        {
            var router = GetRouter();
            var invalidCoordinates = new HashSet<int>(coordinates.Length);
            var augmentedWeightHandler = router.GetAugmentedWeightHandler(DefaultProfile);

            var result = router
                .CalculateWeight(DefaultProfile, augmentedWeightHandler, coordinates, invalidCoordinates)
                .Select(x => x.Select(y => y.Time).ToArray())
                .ToArray();
            return result;
        }

        #endregion

        #region Resolve

        public static Coordinate Resolve(DbGeography originalCoordinate, int? districtId)
        {
            return Resolve(originalCoordinate.ToItineroCoordinate(), districtId);
        }

        public static Coordinate Resolve(Coordinate originalCoordinate, int? districtId)
        {
            var routerPoint = GetRouter(districtId).Resolve(DefaultProfile, originalCoordinate);
            return routerPoint.LocationOnNetwork(GetRouterDb(districtId));
        }

        #endregion
    }
}