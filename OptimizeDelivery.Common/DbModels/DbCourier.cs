using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.DbModels
{
    [Table("Courier")]
    public class DbCourier : IDisposable
    {
        public int Id { get; set; }

        public int? TelegramId { get; set; }

        [Required] [StringLength(50)] public string Name { get; set; }

        [StringLength(50)] public string Surname { get; set; }

        public ICollection<DbTimetableDay> WorkingDays { get; set; }

        public virtual ICollection<DbRoute> Routes { get; set; }

        public void Dispose()
        {
        }
    }
}