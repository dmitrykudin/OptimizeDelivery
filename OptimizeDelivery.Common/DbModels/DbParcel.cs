using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace Common.DbModels
{
    [Table("Parcel")]
    public class DbParcel : IDisposable
    {
        public int Id { get; set; }

        public int DepotId { get; set; }

        public int? RouteId { get; set; }

        public int? DistrictId { get; set; }

        public int? RoutePosition { get; set; }

        [Required]
        public DbGeography OriginalLocation { get; set; }

        [Required]
        public DbGeography RoutableLocation { get; set; }

        public double? Weight { get; set; }

        public double? Volume { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DeliveryDateTimeFromUtc { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DeliveryDateTimeToUtc { get; set; }

        public virtual DbDepot Depot { get; set; }

        public virtual DbRoute Route { get; set; }

        public virtual DbDistrict District { get; set; }

        public void Dispose()
        {
            Depot?.Dispose();
        }
    }
}