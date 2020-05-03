using System;
using Common.DbModels;
using Common.Models.FilterModels;

namespace Common.Abstractions.Repositories
{
    public interface ICourierRepository
    {
        int CreateCourier(DbCourier courier);

        DbCourier GetCourier(int courierId);

        DbCourier[] GetCouriers(CourierFilter filter);
    }
}