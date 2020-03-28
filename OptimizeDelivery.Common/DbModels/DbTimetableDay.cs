using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.DbModels
{
    [Table("TimetableDay")]
    public class DbTimetableDay
    {
        public int Id { get; set; }
        
        public int TimetableId { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public int DayOfWeek { get; set; }

        public bool IsWeekend { get; set; }

        public virtual DbTimetable Timetable { get; set; }
    }
}