using Common.DbModels;
using Common.Models.FilterModels;

namespace Common.Abstractions.Repositories
{
    public interface IParcelRepository
    {
        int CreateParcel(DbParcel parcel);

        void UpdateParcelsRoute(int routeId, (int, int)[] parcelIdWithPosition);

        DbParcel GetParcel(int parcelId);

        DbParcel[] GetParcels(ParcelFilter filter);
    }
}