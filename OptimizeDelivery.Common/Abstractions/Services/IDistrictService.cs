using Common.Models.BusinessModels;

namespace Common.Abstractions.Services
{
    public interface IDistrictService
    {
        District CreateDistrict(District district);

        District GetDistrict(int districtId);

        District[] GetAllDistricts();
    }
}