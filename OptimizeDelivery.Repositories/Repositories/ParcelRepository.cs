using System;
using System.Linq;
using Common.Abstractions.Repositories;
using Common.DbModels;
using Common.Models.FilterModels;

namespace OptimizeDelivery.DataAccessLayer.Repositories
{
    public class ParcelRepository : IParcelRepository
    {
        public int CreateParcel(DbParcel parcel)
        {
            using (var context = new OptimizeDeliveryContext())
            {
                var parcelFromDb = context
                    .Set<DbParcel>()
                    .Add(parcel);

                context.SaveChanges();

                return parcelFromDb.Id;
            }
        }

        public void UpdateParcelsRoute(int routeId, (int, int)[] parcelIdWithPosition)
        {
            using (var context = new OptimizeDeliveryContext())
            {
                var parcelIds = parcelIdWithPosition.Select(x => x.Item1);

                var parcelsFromDb = context
                    .Set<DbParcel>()
                    .Where(x => parcelIds.Contains(x.Id))
                    .ToArray();

                foreach (var parcelFromDb in parcelsFromDb)
                {
                    parcelFromDb.RouteId = routeId;
                    parcelFromDb.RoutePosition = parcelIdWithPosition
                        .FirstOrDefault(x => x.Item1 == parcelFromDb.Id)
                        .Item2;
                }

                context.SaveChanges();
            }
        }

        public DbParcel GetParcel(int parcelId)
        {
            using (var context = new OptimizeDeliveryContext())
            {
                return context
                    .Set<DbParcel>()
                    .FirstOrDefault(x => x.Id == parcelId);
            }
        }

        public DbParcel[] GetParcels(ParcelFilter filter)
        {
            using (var context = new OptimizeDeliveryContext())
            {
                var getParcelsQuery = context
                    .Set<DbParcel>()
                    .AsQueryable();

                if (filter?.RouteId != null && filter.RouteId.IsSet)
                {
                    getParcelsQuery = getParcelsQuery
                        .Where(x => x.RouteId == filter.RouteId.Value);
                }
                
                if (filter?.DistrictId != null && filter.DistrictId.IsSet)
                {
                    getParcelsQuery = getParcelsQuery
                        .Where(x => x.DistrictId.HasValue && x.DistrictId == filter.DistrictId.Value);
                }

                if (filter?.DeliveryDate != null && filter.DeliveryDate.IsSet)
                {
                    if (filter.DeliveryDate.Value.HasValue)
                    {
                        var nextDay = filter.DeliveryDate.Value.Value.AddDays(1);
                        getParcelsQuery = getParcelsQuery
                            .Where(x => x.DeliveryDateTimeFromUtc >= filter.DeliveryDate.Value
                                        && x.DeliveryDateTimeToUtc < nextDay);
                    }
                    else
                    {
                        getParcelsQuery = getParcelsQuery
                            .Where(x => x.DeliveryDateTimeFromUtc == null && x.DeliveryDateTimeToUtc == null);
                    }
                }

                return getParcelsQuery
                    .ToArray();
            }
        }
    }
}