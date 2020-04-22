using System;
using Common.Models.BusinessModels;

namespace Common.Abstractions.Services
{
    public interface ICourierService
    {
        Courier CreateCourier(Courier courier);

        Courier[] GetCouriers(int workingDistrictId, DateTime dateTime);
    }
}