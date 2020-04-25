using System.Linq;
using Common.Abstractions.Repositories;
using Common.DbModels;

namespace OptimizeDelivery.DataAccessLayer.Repositories
{
    public class DistrictRepository : IDistrictRepository
    {
        public int CreateDistrict(DbDistrict district)
        {
            using (var context = new OptimizeDeliveryContext())
            {
                var districtFromDb = context
                    .Set<DbDistrict>()
                    .Add(district);

                context.SaveChanges();

                return districtFromDb.Id;
            }
        }

        public DbDistrict GetDistrict(int districtId)
        {
            using (var context = new OptimizeDeliveryContext())
            {
                return context
                    .Set<DbDistrict>()
                    .FirstOrDefault(x => x.Id == districtId);
            }
        }

        public DbDistrict GetDistrict(string districtName)
        {
            using (var context = new OptimizeDeliveryContext())
            {
                return context
                    .Set<DbDistrict>()
                    .FirstOrDefault(x => x.Name == districtName);
            }
        }

        public DbDistrict[] GetAllDistricts()
        {
            using (var context = new OptimizeDeliveryContext())
            {
                return context
                    .Set<DbDistrict>()
                    .ToArray();
            }
        }
    }
}