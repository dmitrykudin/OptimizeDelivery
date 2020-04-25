using Common.DbModels;

namespace Common.Abstractions.Repositories
{
    public interface IDistrictRepository
    {
        int CreateDistrict(DbDistrict district);

        DbDistrict GetDistrict(int districtId);

        DbDistrict GetDistrict(string districtName);

        DbDistrict[] GetAllDistricts();
    }
}