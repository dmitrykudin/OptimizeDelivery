using System;
using System.Linq;
using Common.Abstractions.Repositories;
using Common.DbModels;

namespace OptimizeDelivery.DataAccessLayer.Repositories
{
    public class DepotRepository : IDepotRepository
    {
        public int CreateDepot(DbDepot depot)
        {
            using (var context = new OptimizeDeliveryContext())
            {
                var depotFromDb = context
                    .Set<DbDepot>()
                    .Add(depot);

                context.SaveChanges();

                return depotFromDb.Id;
            }
        }

        public DbDepot GetDepot(int depotId)
        {
            using (var context = new OptimizeDeliveryContext())
            {
                return context
                    .Set<DbDepot>()
                    .FirstOrDefault(x => x.Id == depotId);
            }
        }

        public DbDepot GetDefaultDepot()
        {
            using (var context = new OptimizeDeliveryContext())
            {
                return context
                    .Set<DbDepot>()
                    .FirstOrDefault();
            }
        }
    }
}