using System;

namespace Common.Models
{
    public class MapLeg
    {
        public int Distance { get; set; }

        public TimeSpan Duration { get; set; }

        public TimeSpan? DurationInTraffic { get; set; }

        public Coordinate StartLocation { get; set; }

        public Coordinate EndLocation { get; set; }

        public string StartAddress { get; set; }

        public string EndAddress { get; set; }

        public DateTime? ArrivalTime { get; set; }

        public DateTime? DepartureTime { get; set; }
    }
}