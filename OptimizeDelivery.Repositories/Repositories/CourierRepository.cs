using System.Data.Entity;
using System.Linq;
using Common.Abstractions.Repositories;
using Common.ConvertHelpers;
using Common.DbModels;
using Common.Models.BusinessModels;

namespace OptimizeDelivery.DataAccessLayer.Repositories
{
    public class CourierRepository : ICourierRepository
    {
        public int CreateCourier(Courier courier)
        {
            using (var context = new OptimizeDeliveryContext())
            {
                var courierFromDb = context
                    .Set<DbCourier>()
                    .Add(courier.ToDbCourier());

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
    }
}