using Common.Models.BusinessModels;

namespace Common.Abstractions.Services
{
    public interface ICourierService
    {
        Courier CreateCourier(Courier courier);
    }
}