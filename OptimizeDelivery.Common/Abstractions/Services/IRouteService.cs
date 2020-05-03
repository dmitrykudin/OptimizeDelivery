using Common.Models.BusinessModels;

namespace Common.Abstractions.Services
{
    public interface IRouteService
    {
        Route CreateRoute(Route route);

        Route GetRoute(int routeId);
    }
}