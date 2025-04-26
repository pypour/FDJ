using Assessment.Contract.Interfaces.Common;
using Assessment.Contract.Models.ExchangeRate;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Net.Http.Json;
using System.Text.Json;

namespace Assessment.Contract.Interfaces.ExchangeRate
{
    // This class can be kind of FACADE, it is a wrapper around the exchange rate API and caches the rates.
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly ICacheService _cacheService;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger _logger;
        private readonly string _baseUrl;
        private readonly string _apiKey;
        private readonly DistributedCacheEntryOptions _cacheOption;

        private string CacheKey
        {
            get { return GetType().Name; }
        }

        public ExchangeRateService(ICacheService cacheService, ExchangeRateConfig config, IHttpClientFactory clientFactory, ILogger<ExchangeRateService> logger)
        {
            _cacheService = cacheService;
            _clientFactory = clientFactory;
            _logger = logger;
            _baseUrl = config.BaseUrl.Trim('/');
            _apiKey = config.ApiKey;
            _cacheOption = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = config.CacheTime
            };
        }

        /// <summary>
        /// Get exchange rates for a given currency from cache or external system.
        /// </summary>
        /// <param name="currency">Input currency to get exchange rates</param>
        public async Task<ConcurrentDictionary<string, decimal>> GetRatesAsync(string currency, CancellationToken cancellationToken)
        {
            return await _cacheService.GetOrAdd($"{CacheKey}-{currency}", async () =>
                {
                    try
                    {
                        var client = _clientFactory.CreateClient("ExchangeRate");
                        var response = await client.GetFromJsonAsync<ExchangeRateResult>($"{_baseUrl}/{_apiKey}/latest/{currency}", cancellationToken);
                        //We can remove this check if we are sure that the API will return null in conversion_rates in other situation.
                        if (response != null && response.Result.Equals("success", StringComparison.InvariantCultureIgnoreCase))
                            return new ConcurrentDictionary<string, decimal>(response.Rates);

                        _logger.LogCritical($"Fetch latest rate of {currency} has been failed. result is {JsonSerializer.Serialize(response)}");
                        return null;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error fetching exchange rates");
                        throw;
                    }
                }, _cacheOption, cancellationToken);
        }
    }
}
