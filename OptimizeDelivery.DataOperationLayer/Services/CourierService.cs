using System;
using System.Data.Entity;
using System.Linq;
using Common;
using Common.DbModels;
using Common.Models.ApiModels;

namespace OptimizeDelivery.DataAccessLayer.Services
{
    public class CourierService
    {
        public CreateCourierResult CreateCourier(CreateCourierRequest request)
        {
            using (var context = new OptimizeDeliveryContext())
            {
                var dbCourier = context
                    .Set<DbCourier>()
                    .FirstOrDefault(x => x.TelegramId == request.TelegramId);

                if (dbCourier != null)
                {
                    return new CreateCourierResult
                    {
                        Id = dbCourier.Id,
                        Status = "Existing",
                    };
                }

                var newCourier = context
                    .Set<DbCourier>()
                    .Add(new DbCourier
                    {
                        TelegramId = request.TelegramId,
                        Name = request.FirstName,
                        Surname = request.LastName,
                    });

                context.SaveChanges();

                return new CreateCourierResult
                {
                    Id = newCourier.Id,
                    Status = "Created",
                };
            }
        }

        public GetRouteResult GetRouteForCourier(GetRouteRequest request)
        {
            var courierFromDb = GetCourierByTelegramId(request.TelegramId);

            if (courierFromDb == null)
            {
                return new GetRouteResult
                {
                    Status = "Unauthorized"
                };
            }

            var routeFromDb = TryAssignRouteForCourier(courierFromDb.Id);
            if (routeFromDb == null)
            {
                return new GetRouteResult
                {
                    Status = "NoRoutes"
                };
            }

            return routeFromDb.ToRouteResult();
        }

        private static DbCourier GetCourierByTelegramId(int telegramId)
        {
            using (var context = new OptimizeDeliveryContext())
            {
                return context
                    .Set<DbCourier>()
                    .FirstOrDefault(x => x.TelegramId == telegramId);
            }
        }

        private static DbRoute TryAssignRouteForCourier(int courierId)
        {
            using (var context = new OptimizeDeliveryContext())
            {
                var today = DateTime.Now.Date;
                var routeForToday = context
                    .Set<DbRoute>()
                    .Include(x => x.Parcels)
                    .FirstOrDefault(x => !x.CourierId.HasValue && DbFunctions.TruncateTime(x.CreationDate) == today);

                if (routeForToday == null)
                {
                    return null;
                }

                routeForToday.CourierId = courierId;

                context.SaveChanges();

                return routeForToday;
            }
        }
    }
}