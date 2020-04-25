using Common.Abstractions.Repositories;
using Common.Abstractions.Services;
using Common.ConvertHelpers;
using Common.Helpers;
using Common.Models.BusinessModels;
using OptimizeDelivery.DataAccessLayer.Repositories;

namespace OptimizeDelivery.Services.Services
{
    public class ParcelService : IParcelService
    {
        public ParcelService()
        {
            ParcelRepository = new ParcelRepository();
        }
        
        public IParcelRepository ParcelRepository { get; set; }
        
        public Parcel CreateParcel(Parcel parcel)
        {
            if (parcel.RoutableLocation == null)
            {
                parcel.RoutableLocation = GeographyHelper
                    .GetDbGeographyPoint(RouterService.Resolve(parcel.OriginalLocation));
            }
            
            var parcelFromDbId = ParcelRepository.CreateParcel(parcel.ToDbParcel());
            var parcelFromDb = ParcelRepository.GetParcel(parcelFromDbId);

            return parcelFromDb.ToParcel();
        }
    }
}