using Common.Abstractions.Repositories;
using Common.Abstractions.Services;
using Common.Models.BusinessModels;
using OptimizeDelivery.DataAccessLayer.Repositories;

namespace OptimizeDelivery.Services.Services
{
    public class DistrictService : IDistrictService
    {
        public IDistrictRepository DistrictRepository { get; set; }

        public DistrictService()
        {
            DistrictRepository = new DistrictRepository();
        }
        
        public District CreateDistrict(District district)
        {
            var districtFromDbId = DistrictRepository.CreateDistrict(district);
            var districtFromDb = DistrictRepository.GetDistrict(districtFromDbId);

            return districtFromDb;
        }
    }
}