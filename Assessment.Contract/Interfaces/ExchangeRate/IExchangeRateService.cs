using Assessment.Contract.Interfaces.Common;
using System.Collections.Concurrent;

namespace Assessment.Contract.Interfaces.ExchangeRate
{
    public interface IExchangeRateService : IService
    {
        /// <summary>
        /// Get the exchange rates for a given currency from external system.
        /// </summary>
        /// <param name="currency">Input currency to get rates</param>
        /// <param name="cancellationToken"></param>
        /// <returns>ConcurrentDictionary of Outpu Currency and exchange rate</returns>
        Task<ConcurrentDictionary<string, decimal>> GetRatesAsync(string currency, CancellationToken cancellationToken);
    }
}
