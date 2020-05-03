using System;

namespace Common.Constants
{
    public static partial class Const
    {
        public static int DefaultParcelsPerDay = 9;

        public static DateTime BaseDateTime = new DateTime(1970, 1, 1, 0, 0, 0);

        public static int DefaultCoordinateSystemId = 4326;

        public static string DefaultCoordinateOutputFormat = "0.0000000000";

        public static string GoogleMapsSharedLinkBaseUrl = "https://www.google.com/maps/dir/?api=1&travelmode=driving";

        public static string GlobalRouterDbFilePath = @"D:/Maps.pbf/RouterDb/saint-petersburg-extended.routerdb";
    }
}