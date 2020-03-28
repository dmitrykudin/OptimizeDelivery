using System.Collections.Generic;

namespace Common.Models.BusinessModels
{
    public class Timetable
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<TimetableDay> TimetableDays { get; set; }
    }
}