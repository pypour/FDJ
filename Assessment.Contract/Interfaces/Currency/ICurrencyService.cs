using Assessment.Contract.Common;
using Assessment.Contract.Interfaces.Common;
using Assessment.Contract.Models;

namespace Assessment.Contract.Interfaces.Currency
{
    public interface ICurrencyService : IService
    {
        Task<ExchangeResult> ConvertAsync(ExchangeRequest request, CancellationToken cancellationToken);
    }
}
