using System.Collections.Generic;
using System.Linq;
using Common.ConvertHelpers;
using Common.DbModels;
using Common.Models.ApiModels;
using Common.Models.BusinessModels;

namespace Common.Helpers
{
    public static class RouteHelper
    {
        public static GetRouteResult ToRouteResult(this DbRoute dbRoute)
        {
            var route = dbRoute.ToRoute();
            var parcels = route.Parcels
                .OrderBy(x => x.RoutePosition.Value)
                .ToArray();

            var legs = route.RouteDetails.Legs.Take(route.RouteDetails.Legs.Count() - 1).ToArray();

            var routeSteps = new List<RouteStep>();

            for (var i = 0; i < parcels.Length; i++)
            {
                var parcel = parcels[i];
                var leg = legs[i];
                var distance = (double) leg.Distance / 1000;
                routeSteps.Add(new RouteStep
                {
                    StepNumber = parcel.RoutePosition.Value,
                    Distance = distance.ToString("0.00km"),
                    DestinationAddress = leg.EndAddress
                });
            }

            return new GetRouteResult
            {
                Status = "OK",
                Steps = routeSteps.ToArray(),
                RouteUrl = GetRouteUrl(parcels)
            };
        }

        private static string GetRouteUrl(Parcel[] parcels)
        {
            var destination = parcels.Last().Location.ToStringNoWhitespace();
            var waypoints = string.Join("|",
                parcels
                    .Take(parcels.Length - 1)
                    .Select(x => x.Location.ToStringNoWhitespace()));
            return string.Join("&",
                Constants.Const.GoogleMapsSharedLinkBaseUrl,
                "destination=" + destination,
                "waypoints=" + waypoints);
        }
    }
}