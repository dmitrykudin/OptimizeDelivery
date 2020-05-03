using Common.Abstractions.Repositories;
using Common.Abstractions.Services;
using Common.ConvertHelpers;
using Common.Helpers;
using Common.Models.BusinessModels;
using OptimizeDelivery.DataAccessLayer.Repositories;

namespace OptimizeDelivery.Services.Services
{
    public class DepotService : IDepotService
    {
        private IDepotRepository DepotRepository { get; set; }

        public DepotService()
        {
            DepotRepository = new DepotRepository();
        }
        
        public Depot CreateDepot(Depot depot)
        {
            if (depot.RoutableLocation == null)
            {
                depot.RoutableLocation = GeographyHelper
                    .GetDbGeographyPoint(ItineroRouter.Resolve(depot.OriginalLocation, null));
            }

            var depotFromDbId = DepotRepository.CreateDepot(depot.ToDbDepot());
            var depotFromDb = DepotRepository.GetDepot(depotFromDbId);

            return depotFromDb.ToDepot();
        }

        public Depot GetDepot(int depotId)
        {
            return DepotRepository
                .GetDepot(depotId)
                .ToDepot();
        }

        public Depot GetDefaultDepot()
        {
            return DepotRepository
                .GetDefaultDepot()
                .ToDepot();
        }
    }
}