using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.DbModels
{
    [Table("Route")]
    public class DbRoute
    {
        public DbRoute()
        {
            Parcels = new List<DbParcel>();
        }

        public int Id { get; set; }

        public int? CourierId { get; set; }

        public string RouteJsonDetails { get; set; }

        public DateTime CreationDate { get; set; }

        public int TotalTime { get; set; }

        public virtual DbCourier Courier { get; set; }

        public virtual ICollection<DbParcel> Parcels { get; set; }
    }
}