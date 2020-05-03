using System.Linq;
using Common.Abstractions.Repositories;
using Common.Abstractions.Services;
using Common.ConvertHelpers;
using Common.Helpers;
using Common.Models.BusinessModels;
using Common.Models.FilterModels;
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
                    .GetDbGeographyPoint(ItineroRouter.Resolve(parcel.OriginalLocation, parcel.DistrictId));
            }
            
            var parcelFromDbId = ParcelRepository.CreateParcel(parcel.ToDbParcel());
            var parcelFromDb = ParcelRepository.GetParcel(parcelFromDbId);

            return parcelFromDb.ToParcel();
        }

        public void UpdateParcelsRoute(int routeId, Parcel[] parcels)
        {
            ParcelRepository.UpdateParcelsRoute(routeId, parcels
                .Where(x => x.RoutePosition.HasValue)
                .Select(x => (x.Id, x.RoutePosition.Value))
                .ToArray());
        }

        public Parcel[] GetParcels(ParcelFilter filter)
        {
            return ParcelRepository
                .GetParcels(filter)
                .Select(x => x.ToParcel())
                .ToArray();
        }
    }
}