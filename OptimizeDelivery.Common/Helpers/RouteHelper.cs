using System.Collections.Generic;
using System.Linq;
using Common.Constants;
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
            return new GetRouteResult
            {
                Status = "OK",
            };
        }

        private static string GetRouteUrl(Parcel[] parcels)
        {
            var destination = parcels.Last().OriginalLocation.ToStringNoWhitespace();
            var waypoints = string.Join("|",
                parcels
                    .Take(parcels.Length - 1)
                    .Select(x => x.OriginalLocation.ToStringNoWhitespace()));
            return string.Join("&",
                Const.GoogleMapsSharedLinkBaseUrl,
                "destination=" + destination,
                "waypoints=" + waypoints);
        }
    }
}