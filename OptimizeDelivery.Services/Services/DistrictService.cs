using Common.Abstractions.Repositories;
using Common.Abstractions.Services;
using Common.Models.BusinessModels;
using OptimizeDelivery.DataAccessLayer.Repositories;

namespace OptimizeDelivery.Services.Services
{
    public class DistrictService : IDistrictService
    {
        public DistrictService()
        {
            DistrictRepository = new DistrictRepository();
        }

        public IDistrictRepository DistrictRepository { get; set; }

        public District CreateDistrict(District district)
        {
            var districtFromDbId = DistrictRepository.CreateDistrict(district);
            var districtFromDb = DistrictRepository.GetDistrict(districtFromDbId);

            return districtFromDb;
        }

        public District[] GetAllDistricts()
        {
            return DistrictRepository.GetAllDistricts();
        }
    }
}