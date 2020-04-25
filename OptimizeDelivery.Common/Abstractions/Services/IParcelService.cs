using Common.Models.BusinessModels;
using Common.Models.FilterModels;

namespace Common.Abstractions.Services
{
    public interface IParcelService
    {
        Parcel CreateParcel(Parcel parcel);

        Parcel[] GetParcels(ParcelFilter filter);
    }
}