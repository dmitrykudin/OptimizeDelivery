using System;
using System.Data.Entity;
using System.Linq;
using Common.Abstractions.Repositories;
using Common.DbModels;

namespace OptimizeDelivery.DataAccessLayer.Repositories
{
    public class CourierRepository : ICourierRepository
    {
        public int CreateCourier(DbCourier courier)
        {
            using (var context = new OptimizeDeliveryContext())
            {
                var courierFromDb = context
                    .Set<DbCourier>()
                    .Add(courier);

                context.SaveChanges();

                return courierFromDb.Id;
            }
        }

        public DbCourier GetCourier(int id)
        {
            using (var context = new OptimizeDeliveryContext())
            {
                return context
                    .Set<DbCourier>()
                    .Include(x => x.WorkingDays)
                    .FirstOrDefault(x => x.Id == id);
            }
        }

        public DbCourier[] GetCouriers(int workingDistrictId, DayOfWeek dayOfWeek)
        {
            using (var context = new OptimizeDeliveryContext())
            {
                return context
                    .Set<DbCourier>()
                    .Where(x => x.WorkingDistrictId == workingDistrictId
                                && x.WorkingDays.FirstOrDefault(y => y.DayOfWeek == (int) dayOfWeek) != null
                                && !x.WorkingDays.FirstOrDefault(y => y.DayOfWeek == (int) dayOfWeek).IsWeekend)
                    .ToArray();
            }
        }
    }
}