using System;
using System.Data.Entity.Spatial;
using Common.Helpers;
using Itinero.LocalGeo;

namespace Common.Models.BusinessModels
{
    public class Parcel
    {
        public int Id { get; set; }

        public int DepotId { get; set; }

        public int? RouteId { get; set; }

        public int? DistrictId { get; set; }

        public int? RoutePosition { get; set; }

        public DbGeography OriginalLocation { get; set; }

        public DbGeography RoutableLocation { get; set; }

        public Coordinate RoutableCoordinate => RoutableLocation.ToItineroCoordinate();

        public double? Weight { get; set; }

        public double? Volume { get; set; }

        public TimeWindow DeliveryTimeWindow { get; set; }
    }
}