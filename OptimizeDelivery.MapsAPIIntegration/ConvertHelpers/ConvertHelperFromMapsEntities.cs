using Common.Constants;
using Common.Models;
using GoogleMapsApi.Entities.Common;
using GoogleMapsApi.Entities.Directions.Response;

namespace OptimizeDelivery.MapsAPIIntegration.ConvertHelpers
{
    public static class ConvertHelperFromMapsEntities
    {
        public static MapLeg ToMapLeg(this Leg leg)
        {
            return leg == null
                ? null
                : new MapLeg
                {
                    Distance = leg.Distance.Value,
                    Duration = leg.Duration.Value,
                    DurationInTraffic = leg.DurationInTraffic?.Value,
                    StartAddress = leg.StartAddress,
                    StartLocation = leg.StartLocation.ToCoordinate(),
                    EndAddress = leg.EndAddress,
                    EndLocation = leg.EndLocation.ToCoordinate(),
                    DepartureTime = Const.BaseDateTime + leg.DepartureTime?.Value,
                    ArrivalTime = Const.BaseDateTime + leg.ArrivalTime?.Value
                };
        }

        public static Coordinate ToCoordinate(this Location location)
        {
            return location == null
                ? null
                : new Coordinate
                {
                    Latitude = location.Latitude,
                    Longitude = location.Longitude
                };
        }
    }
}