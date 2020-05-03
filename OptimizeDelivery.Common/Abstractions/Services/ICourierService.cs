using Common.Models.BusinessModels;
using Common.Models.FilterModels;

namespace Common.Abstractions.Services
{
    public interface ICourierService
    {
        Courier CreateCourier(Courier courier);

        Courier[] GetCouriers(CourierFilter filter);
    }
}