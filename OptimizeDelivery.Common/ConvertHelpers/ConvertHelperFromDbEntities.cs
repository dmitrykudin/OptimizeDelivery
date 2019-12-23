﻿using System.Collections.Generic;
using System.Linq;
using Common.DbModels;
using Common.Models;
using Common.Models.ApiModels;
using Common.Models.DbMappedModels;
using Newtonsoft.Json;

namespace Common.ConvertHelpers
{
    public static class ConvertHelperFromDbEntities
    {
        public static Parcel ToParcel(this DbParcel dbParcel)
        {
            return dbParcel == null
                ? null
                : new Parcel
                {
                    Id = dbParcel.Id,
                    Location = dbParcel.Location,
                    RoutePosition = dbParcel.RoutePosition,
                    DeliveryDateTimeFromUtc = dbParcel.DeliveryDateTimeFromUtc,
                    DeliveryDateTimeToUtc = dbParcel.DeliveryDateTimeToUtc
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
                    Surname = dbCourier.Surname
                };
        }
    }
}