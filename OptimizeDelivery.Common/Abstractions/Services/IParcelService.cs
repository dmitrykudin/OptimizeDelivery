using Common.Models.BusinessModels;

namespace Common.Abstractions.Services
{
    public interface IParcelService
    {
        Parcel CreateParcel(Parcel parcel);
    }
}