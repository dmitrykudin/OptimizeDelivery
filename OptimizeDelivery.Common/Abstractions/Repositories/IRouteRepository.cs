using Common.DbModels;

namespace Common.Abstractions.Repositories
{
    public interface IRouteRepository
    {
        int CreateRoute(DbRoute route);

        DbRoute GetRoute(int routeId);
    }
}