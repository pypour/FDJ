using Assessment.Contract.Interfaces.Currency;
using Assessment.Contract.Interfaces.ExchangeRate;
using Assessment.Contract.Models;

namespace Assessment.Service.Currency
{
    public class CurrencyService(IExchangeRateService exchangeRateService) : ICurrencyService
    {
        public string CacheKey
        {
            get
            {
                return GetType().Name;
            }
        }

        public async Task<ExchangeResult> ConvertAsync(ExchangeRequest request, CancellationToken cancellationToken)
        {
            var rates = await exchangeRateService.GetRatesAsync(request.InputCurrency, cancellationToken);

            if (rates == null)
            {
                throw new Exception("Can not fetch rates");
            }

            if (rates.TryGetValue(request.OutputCurrency, out var rate))
            {
                return new ExchangeResult(request)
                {
                    Value = rates[request.OutputCurrency] * request.Amount,
                };
            }

            throw new Exception($"Can not find rate of {request.InputCurrency} to {request.OutputCurrency}.");
        }
    }
}
