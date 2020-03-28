using System;
using System.Data.Entity;
using System.Linq;
using Common;
using Common.Abstractions.Repositories;
using Common.Abstractions.Services;
using Common.ConvertHelpers;
using Common.DbModels;
using Common.Models.ApiModels;
using Common.Models.BusinessModels;
using OptimizeDelivery.DataAccessLayer;
using OptimizeDelivery.DataAccessLayer.Repositories;

namespace OptimizeDelivery.Services.Services
{
    public class CourierService : ICourierService
    {
        private ICourierRepository CourierRepository { get; set; }
        
        private ITimetableRepository TimetableRepository { get; set; }
        
        public CourierService()
        {
            CourierRepository = new CourierRepository();
            TimetableRepository = new TimetableRepository();
        }

        public Courier CreateCourier(Courier courier)
        {
            var timetableFromDb = TimetableRepository.CreateTimetable(courier.Timetable);
            courier.Timetable.Id = timetableFromDb.Id;

            var courierFromDbId = CourierRepository.CreateCourier(courier);
            var courierFromDb = CourierRepository.GetCourier(courierFromDbId);

            return courierFromDb.ToCourier();
        }

        public CreateCourierResult CreateCourier(CreateCourierRequest request)
        {
            using (var context = new OptimizeDeliveryContext())
            {
                var dbCourier = context
                    .Set<DbCourier>()
                    .FirstOrDefault(x => x.TelegramId == request.TelegramId);

                if (dbCourier != null)
                    return new CreateCourierResult
                    {
                        Id = dbCourier.Id,
                        Status = "Existing"
                    };

                var newCourier = context
                    .Set<DbCourier>()
                    .Add(new DbCourier
                    {
                        TelegramId = request.TelegramId,
                        Name = request.FirstName,
                        Surname = request.LastName
                    });

                context.SaveChanges();

                return new CreateCourierResult
                {
                    Id = newCourier.Id,
                    Status = "Created"
                };
            }
        }

        public GetRouteResult GetRouteForCourier(GetRouteRequest request)
        {
            var courierFromDb = GetCourierByTelegramId(request.TelegramId);

            if (courierFromDb == null)
                return new GetRouteResult
                {
                    Status = "Unauthorized"
                };

            var routeFromDb = TryAssignRouteForCourier(courierFromDb.Id);
            if (routeFromDb == null)
                return new GetRouteResult
                {
                    Status = "NoRoutes"
                };

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

                if (routeForToday == null) return null;

                routeForToday.CourierId = courierId;

                context.SaveChanges();

                return routeForToday;
            }
        }
    }
}