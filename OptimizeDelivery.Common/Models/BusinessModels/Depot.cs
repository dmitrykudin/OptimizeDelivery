using System.Data.Entity.Spatial;
using Common.Helpers;
using Itinero.LocalGeo;

namespace Common.Models.BusinessModels
{
    public class Depot
    {
        public int Id { get; set; }

        public DbGeography OriginalLocation { get; set; }

        public DbGeography RoutableLocation { get; set; }

        public Coordinate RoutableCoordinate => RoutableLocation.ToItineroCoordinate();

        public WorkingWindow WorkingTimeWindow { get; set; }
    }
}