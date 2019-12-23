using System;
using System.Data.Entity.Spatial;

namespace Common.Models.DbMappedModels
{
    public class Parcel
    {
        public int Id { get; set; }

        public int? RoutePosition { get; set; }

        public DbGeography Location { get; set; }

        public DateTime DeliveryDateTimeFromUtc { get; set; }

        public DateTime DeliveryDateTimeToUtc { get; set; }

        public Depot Depot { get; set; }
    }
}