using System.Text.Json.Serialization;

namespace Assessment.Contract.Models.ExchangeRate
{
    public class ExchangeRateResult
    {
		[JsonPropertyName("result")]
		public string	Result { get; set; }

        [JsonPropertyName("conversion_rates")]
        public Dictionary<string, decimal> Rates { get; set; }
    }
}
