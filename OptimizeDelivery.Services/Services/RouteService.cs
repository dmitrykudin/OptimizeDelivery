using Common.Abstractions.Repositories;
using Common.Abstractions.Services;
using Common.ConvertHelpers;
using Common.Models.BusinessModels;
using OptimizeDelivery.DataAccessLayer.Repositories;

namespace OptimizeDelivery.Services.Services
{
    public class RouteService : IRouteService
    {
        private IRouteRepository RouteRepository { get; set; }

        public RouteService()
        {
            RouteRepository = new RouteRepository();
        }
        
        public Route CreateRoute(Route route)
        {
            var routeFromDbId = RouteRepository.CreateRoute(route.ToDbRoute());
            var routeFromDb = RouteRepository.GetRoute(routeFromDbId);

            return routeFromDb.ToRoute();
        }

        public Route GetRoute(int routeId)
        {
            return RouteRepository
                .GetRoute(routeId)
                .ToRoute();
        }
    }
}