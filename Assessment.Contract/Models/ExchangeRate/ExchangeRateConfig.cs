namespace Assessment.Contract.Models.ExchangeRate
{
    public class ExchangeRateConfig
    {
        /// <summary>
        /// Base URL for the exchange rate API.
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// API key for the exchange rate API.
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Cache time for the exchange rate data to prevent sending more request to ExchangeAPI.
        /// </summary>
        public TimeSpan CacheTime { get; set; }
    }
}
