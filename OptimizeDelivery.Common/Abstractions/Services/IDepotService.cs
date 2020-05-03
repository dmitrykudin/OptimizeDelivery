using Common.Models.BusinessModels;

namespace Common.Abstractions.Services
{
    public interface IDepotService
    {
        Depot CreateDepot(Depot depot);

        Depot GetDepot(int depotId);

        Depot GetDefaultDepot();
    }
}