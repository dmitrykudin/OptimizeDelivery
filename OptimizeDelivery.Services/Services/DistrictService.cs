using System.Linq;
using Common.Abstractions.Repositories;
using Common.Abstractions.Services;
using Common.ConvertHelpers;
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
            var districtFromDbId = DistrictRepository.CreateDistrict(district.ToDbDistrict());
            var districtFromDb = DistrictRepository.GetDistrict(districtFromDbId);

            return districtFromDb.ToDistrict();
        }

        public District GetDistrict(int districtId)
        {
            return DistrictRepository.GetDistrict(districtId).ToDistrict();
        }

        public District[] GetAllDistricts()
        {
            return DistrictRepository
                .GetAllDistricts()
                .Select(x => x.ToDistrict())
                .ToArray();
        }
    }
}