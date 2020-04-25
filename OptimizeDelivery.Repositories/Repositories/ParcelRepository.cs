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
            throw new NotImplementedException();
        }
    }
}