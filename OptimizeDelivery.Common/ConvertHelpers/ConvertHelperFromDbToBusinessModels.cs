using System;
using System.Linq;
using Common.DbModels;
using Common.Models;
using Common.Models.BusinessModels;
using NetTopologySuite.IO;
using Newtonsoft.Json;

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
                    DeliveryTimeWindow = new TimeWindow(dbParcel.DeliveryDateTimeFromUtc, dbParcel.DeliveryDateTimeToUtc),
                };
        }

        public static Depot ToDepot(this DbDepot dbDepot)
        {
            return dbDepot == null
                ? null
                : new Depot
                {
                    Id = dbDepot.Id,
                    Location = dbDepot.Location
                };
        }

        public static Route ToRoute(this DbRoute dbRoute)
        {
            return dbRoute == null
                ? null
                : new Route
                {
                    Id = dbRoute.Id,
                    Parcels = dbRoute.Parcels.Select(x => x.ToParcel()),
                    RouteDetails = !string.IsNullOrEmpty(dbRoute.RouteDetails)
                        ? JsonConvert.DeserializeObject<MapRouteDetails>(dbRoute.RouteDetails)
                        : null
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
                    RouterDbFilePath = dbDistrict.RouterDbFilePath,
                };
        }
    }
}