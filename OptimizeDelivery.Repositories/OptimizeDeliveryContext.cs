using System.Data.Entity;
using Common.DbModels;

namespace OptimizeDelivery.DataAccessLayer
{
    public class OptimizeDeliveryContext : DbContext
    {
        public OptimizeDeliveryContext() : base("name=OptimizeDelivery")
        {
        }

        public virtual DbSet<DbCourier> Couriers { get; set; }

        public virtual DbSet<DbParcel> Parcels { get; set; }

        public virtual DbSet<DbDepot> Depots { get; set; }

        public virtual DbSet<DbRoute> Routes { get; set; }

        public virtual DbSet<DbTimetableDay> DbTimetableDays { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbParcel>()
                .HasOptional(x => x.Route)
                .WithMany(y => y.Parcels)
                .HasForeignKey(x => x.RouteId);

            modelBuilder.Entity<DbParcel>()
                .HasRequired(x => x.Depot)
                .WithMany(y => y.Parcels)
                .HasForeignKey(x => x.DepotId);

            modelBuilder.Entity<DbTimetableDay>()
                .HasRequired(x => x.Courier)
                .WithMany(x => x.WorkingDays)
                .HasForeignKey(x => x.CourierId);
        }
    }
}