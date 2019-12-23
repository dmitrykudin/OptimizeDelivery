﻿using System;
using System.Data.Entity.Spatial;

namespace Common
{
    public static partial class Constants
    {
        public static int DefaultParcelsPerDay = 9;

        public static DateTime BaseDateTime = new DateTime(1970, 1, 1, 0, 0, 0);

        public static int DefaultCoordinateSystemId = 4326;

        public static string DefaultCoordinateOutputFormat = "0.0000000000";

        public static DbGeography DefaultDepotCoordinate =
            DbGeography.FromText(GeographyHelper.GetPointString(59.796169, 30.402962));

        public static string GoogleMapsSharedLinkBaseUrl = "https://www.google.com/maps/dir/?api=1&travelmode=driving";
    }
}