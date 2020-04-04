using Common.Models.BusinessModels;

namespace Common.Abstractions.Repositories
{
    public interface IDistrictRepository
    {
        int CreateDistrict(District district);

        District GetDistrict(int districtId);
    }
}