using System;
using System.Data.Entity;
using System.Linq;
using Common.Abstractions.Repositories;
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
                    .Add(new DbCourier
                    {
                        Name = courier.Name,
                        Surname = courier.Surname,
                        TimetableId = courier.Timetable.Id,
                    });

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
                    .Include(x => x.Timetable)
                    .Include(x => x.Timetable.TimetableDays)
                    .FirstOrDefault(x => x.Id == id);
            }
        }
    }
}