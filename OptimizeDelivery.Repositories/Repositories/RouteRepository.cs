using System.Linq;
using Common.Abstractions.Repositories;
using Common.DbModels;

namespace OptimizeDelivery.DataAccessLayer.Repositories
{
    public class RouteRepository : IRouteRepository
    {
        public int CreateRoute(DbRoute route)
        {
            using (var context = new OptimizeDeliveryContext())
            {
                var routeFromDb = context
                    .Set<DbRoute>()
                    .Add(route);

                context.SaveChanges();

                return routeFromDb.Id;
            }
        }

        public DbRoute GetRoute(int routeId)
        {
            using (var context = new OptimizeDeliveryContext())
            {
                return context
                    .Set<DbRoute>()
                    .FirstOrDefault(x => x.Id == routeId);
            }
        }
    }
}