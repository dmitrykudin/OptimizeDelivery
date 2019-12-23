﻿using System.Collections.Generic;
using Common.Models.DbMappedModels;

namespace Common.Models
{
    public class MapRoute
    {
        public IEnumerable<Parcel> Parcels { get; set; }

        public MapRouteDetails RouteDetails { get; set; }

        public string RouteDetailsSerialized { get; set; }
    }
}