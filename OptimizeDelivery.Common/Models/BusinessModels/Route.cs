using System;

namespace Common.Models.BusinessModels
{
    public class Route
    {
        public int Id { get; set; }

        public int? CourierId { get; set; }

        public string RouteJsonDetails { get; set; }

        public DateTime CreationDate { get; set; }

        public int TotalTime { get; set; }
    }
}