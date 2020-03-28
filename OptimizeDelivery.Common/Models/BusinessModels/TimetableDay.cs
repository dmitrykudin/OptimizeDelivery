using System;

namespace Common.Models.BusinessModels
{
    public class TimetableDay
    {
        public int Id { get; set; }

        public int TimetableId { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public DayOfWeek DayOfWeek { get; set; }

        public bool IsWeekend { get; set; }
    }
}