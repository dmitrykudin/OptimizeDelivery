using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace Common.DbModels
{
    [Table("Depot")]
    public class DbDepot : IDisposable
    {
        public int Id { get; set; }

        [Required] 
        public DbGeography Location { get; set; }

        public virtual ICollection<DbParcel> Parcels { get; set; }

        public void Dispose()
        {
        }
    }
}