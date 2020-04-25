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

                if (filter?.DistrictId != null)
                {
                    getParcelsQuery = getParcelsQuery
                        .Where(x => x.DistrictId.HasValue && x.DistrictId == filter.DistrictId);
                }

                if (filter?.DeliveryDate != null)
                {
                    getParcelsQuery = getParcelsQuery
                        .Where(x => x.DeliveryDateTimeFromUtc >= filter.DeliveryDate 
                                    && x.DeliveryDateTimeToUtc <= filter.DeliveryDate);
                }

                return getParcelsQuery
                    .ToArray();
            }
        }
    }
}