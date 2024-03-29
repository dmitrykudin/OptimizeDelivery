﻿using System;
using System.Collections.Generic;
using System.Linq;
using Common.DbModels;
using Common.Models;
using Common.Models.BusinessModels;
using Common.Models.ServiceModels;
using Newtonsoft.Json;
using OptimizeDelivery.DataAccessLayer;
using OptimizeDelivery.MapsAPIIntegration;

namespace OptimizeDelivery.Services.Services
{
    public class OptimizeDeliveryService
    {
        #region Old

        public MapRoute[] MakeRoutesForClusters(DeliveryCluster[] clusters, Depot depot)
        {
            var directionsHelper = new DirectionsHelper();
            var mapRoutes = new List<MapRoute>();

            foreach (var cluster in clusters)
            {
                var mapRoute = directionsHelper.RequestRoutesPerCluster(cluster, depot).Result;
                if (mapRoute != null) mapRoutes.Add(mapRoute);
            }

            var mapRoutesArray = mapRoutes.ToArray();
            SaveMapRoutes(mapRoutesArray);

            return mapRoutesArray;
        }

        private void SaveMapRoutes(MapRoute[] mapRoutes)
        {
            var today = DateTime.Now.Date;
            using (var context = new OptimizeDeliveryContext())
            {
                foreach (var mapRoute in mapRoutes)
                {
                    var dbRoute = context
                        .Set<DbRoute>()
                        .Add(new DbRoute
                        {
                            RouteJsonDetails = JsonConvert.SerializeObject(mapRoute.RouteDetails),
                            CreationDate = today
                        });

                    context.SaveChanges();

                    var parcelIds = mapRoute.Parcels.Select(x => x.Id).ToArray();

                    var dbParcels = context
                        .Set<DbParcel>()
                        .Where(x => parcelIds.Contains(x.Id));

                    foreach (var parcel in mapRoute.Parcels)
                    {
                        var dbParcel = dbParcels.FirstOrDefault(x => x.Id == parcel.Id);
                        dbParcel.RouteId = dbRoute.Id;
                        dbParcel.RoutePosition = parcel.RoutePosition;
                    }

                    context.SaveChanges();
                }
            }
        }

        #endregion
    }
}