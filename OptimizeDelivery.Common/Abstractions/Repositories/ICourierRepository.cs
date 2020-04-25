using System;
using Common.DbModels;

namespace Common.Abstractions.Repositories
{
    public interface ICourierRepository
    {
        int CreateCourier(DbCourier courier);

        DbCourier GetCourier(int id);

        DbCourier[] GetCouriers(int workingDistrictId, DayOfWeek dayOfWeek);
    }
}