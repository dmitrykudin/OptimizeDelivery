using System;
using System.Linq;
using Common.DbModels;
using Common.Models.BusinessModels;
using NetTopologySuite.IO;

namespace Common.ConvertHelpers
{
    public static class ConvertHelperFromDbToBusinessModels
    {
        public static Parcel ToParcel(this DbParcel dbParcel)
        {
            return dbParcel == null
                ? null
                : new Parcel
                {
                    Id = dbParcel.Id,
                    DepotId = dbParcel.DepotId,
                    RouteId = dbParcel.RouteId,
                    DistrictId = dbParcel.DistrictId,
                    RoutePosition = dbParcel.RoutePosition,
                    OriginalLocation = dbParcel.OriginalLocation,
                    RoutableLocation = dbParcel.RoutableLocation,
                    Weight = dbParcel.Weight,
                    Volume = dbParcel.Volume,
                    DeliveryTimeWindow =
                        new TimeWindow(dbParcel.DeliveryDateTimeFromUtc, dbParcel.DeliveryDateTimeToUtc)
                };
        }

        public static Depot ToDepot(this DbDepot dbDepot)
        {
            return dbDepot == null
                ? null
                : new Depot
                {
                    Id = dbDepot.Id,
                    OriginalLocation = dbDepot.OriginalLocation,
                    RoutableLocation = dbDepot.RoutableLocation,
                    WorkingTimeWindow = new WorkingWindow(dbDepot.WorkingTimeFromUtc, dbDepot.WorkingTimeToUtc)
                };
        }

        public static Route ToRoute(this DbRoute dbRoute)
        {
            return dbRoute == null
                ? null
                : new Route
                {
                    Id = dbRoute.Id,
                    CourierId = dbRoute.CourierId,
                    TotalTime = dbRoute.TotalTime,
                    CreationDate = dbRoute.CreationDate,
                    RouteJsonDetails = dbRoute.RouteJsonDetails
                };
        }

        public static Courier ToCourier(this DbCourier dbCourier)
        {
            return dbCourier == null
                ? null
                : new Courier
                {
                    Id = dbCourier.Id,
                    Name = dbCourier.Name,
                    Surname = dbCourier.Surname,
                    WorkingDistrict = dbCourier.WorkingDistrict.ToDistrict(),
                    WorkingDays = dbCourier.WorkingDays
                        .Select(x => new TimetableDay
                        {
                            StartTime = x.StartTime,
                            EndTime = x.EndTime,
                            DayOfWeek = (DayOfWeek) x.DayOfWeek,
                            IsWeekend = x.IsWeekend
                        })
                };
        }

        public static District ToDistrict(this DbDistrict dbDistrict)
        {
            var wkbReader = new WKBReader();

            return dbDistrict == null
                ? null
                : new District
                {
                    Id = dbDistrict.Id,
                    Name = dbDistrict.Name,
                    Area = wkbReader.Read(dbDistrict.Area.AsBinary()),
                    RouterDbFilePath = dbDistrict.RouterDbFilePath
                };
        }
    }
}