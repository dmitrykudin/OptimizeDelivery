using System.Collections.Generic;

namespace Common.Models.DbMappedModels
{
    public class Route
    {
        public int Id { get; set; }

        public MapRouteDetails RouteDetails { get; set; }

        public Courier Courier { get; set; }

        public IEnumerable<Parcel> Parcels { get; set; }
    }
}