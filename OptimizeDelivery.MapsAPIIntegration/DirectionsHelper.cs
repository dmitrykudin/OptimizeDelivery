using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Models;
using Common.Models.DbMappedModels;
using Common.Models.ServiceModels;
using GoogleMapsApi;
using GoogleMapsApi.Entities.Directions.Request;
using GoogleMapsApi.Entities.Directions.Response;
using OptimizeDelivery.MapsAPIIntegration.ConvertHelpers;

namespace OptimizeDelivery.MapsAPIIntegration
{
    public class DirectionsHelper
    {
        private static string ApiKey => ConfigurationManager.AppSettings["GoogleMapsApiKey"];

        private static DirectionsRequest CreateRequest(string origin, string destination, string[] waypoints,
            DateTime departureTime)
        {
            return new DirectionsRequest
            {
                ApiKey = ApiKey,
                Language = "ru",
                OptimizeWaypoints = true,
                TravelMode = TravelMode.Driving,
                Origin = origin,
                Destination = destination,
                Waypoints = waypoints,
                DepartureTime = departureTime
            };
        }

        private async Task<DirectionsResponse> CreateRoute(string origin, string destination, string[] waypoints,
            DateTime departureTime)
        {
            var request = CreateRequest(origin, destination, waypoints, departureTime);
            return await GoogleMaps.Directions.QueryAsync(request);
        }

        public async Task<MapRoute> RequestRoutesPerCluster(DeliveryCluster cluster, Depot depot)
        {
            var depotLocationString = depot.Location.ToStringNoWhitespace();
            var mapRouteLocationStrings = cluster.Parcels
                .Select(x => x.Location.ToStringNoWhitespace())
                .ToArray();
            var directionsResponse = await CreateRoute(depotLocationString, depotLocationString,
                mapRouteLocationStrings, DateTime.Now.AddDays(1));
            var primaryRoute = directionsResponse.Routes.FirstOrDefault();

            for (var i = 0; i < primaryRoute.WaypointOrder.Length; i++)
            {
                var position = primaryRoute.WaypointOrder[i];
                cluster.Parcels[position].RoutePosition = i;
            }

            cluster.Parcels = cluster.Parcels.OrderBy(x => x.RoutePosition.Value).ToArray();

            return primaryRoute == null
                ? null
                : new MapRoute
                {
                    Parcels = cluster.Parcels,
                    RouteDetails = new MapRouteDetails
                    {
                        Legs = primaryRoute.Legs.Select(x => x.ToMapLeg())
                    }
                };
        }
    }
}