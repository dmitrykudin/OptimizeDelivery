using System;

namespace Common.Models.BusinessModels
{
    public class TimetableDay
    {
        public int TimetableId { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public int DayOfWeek { get; set; }
    }
}