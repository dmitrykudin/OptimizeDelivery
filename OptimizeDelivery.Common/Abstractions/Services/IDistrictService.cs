using Common.Models.BusinessModels;

namespace Common.Abstractions.Services
{
    public interface IDistrictService
    {
        District CreateDistrict(District district);

        District[] GetAllDistricts();
    }
}