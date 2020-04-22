using System;
using Common.DbModels;
using Common.Models.BusinessModels;

namespace Common.Abstractions.Repositories
{
    public interface ICourierRepository
    {
        int CreateCourier(Courier courier);

        DbCourier GetCourier(int id);

        DbCourier[] GetCouriers(int workingDistrictId, DayOfWeek dayOfWeek);
    }
}