using Common.DbModels;

namespace Common.Abstractions.Repositories
{
    public interface IDepotRepository
    {
        int CreateDepot(DbDepot depot);

        DbDepot GetDepot(int depotId);

        DbDepot GetDefaultDepot();
    }
}