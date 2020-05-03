using System.Data.Entity;
using System.Linq;
using Common.Abstractions.Repositories;
using Common.DbModels;
using Common.Models.FilterModels;

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

        public DbCourier GetCourier(int courierId)
        {
            using (var context = new OptimizeDeliveryContext())
            {
                return context
                    .Set<DbCourier>()
                    .Include(x => x.WorkingDays)
                    .FirstOrDefault(x => x.Id == courierId);
            }
        }

        public DbCourier[] GetCouriers(CourierFilter filter)
        {
            using (var context = new OptimizeDeliveryContext())
            {
                var couriersQuery = context
                    .Set<DbCourier>()
                    .AsQueryable();

                if (filter.WorkingDistrictId.HasValue)
                    couriersQuery = couriersQuery
                        .Where(x => x.WorkingDistrictId == filter.WorkingDistrictId.Value);

                if (filter.WorkingDay.HasValue)
                {
                    var dayOfWeek = filter.WorkingDay.Value.DayOfWeek;
                    couriersQuery = couriersQuery
                        .Where(x => x.WorkingDays.FirstOrDefault(y => y.DayOfWeek == (int) dayOfWeek) != null
                                    && !x.WorkingDays.FirstOrDefault(y => y.DayOfWeek == (int) dayOfWeek).IsWeekend);
                }

                return couriersQuery
                    .ToArray();
            }
        }
    }
}