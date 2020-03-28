using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.DbModels
{
    [Table("Timetable")]
    public class DbTimetable
    {
        public int Id { get; set; }

        [StringLength(50)] public string Name { get; set; }

        public ICollection<DbTimetableDay> TimetableDays { get; set; }
    }
}