namespace Assessment.Contract.Models
{
    public class ExchangeResult : ExchangeRequest
    {
        public ExchangeResult(ExchangeRequest request)
        {
            InputCurrency = request.InputCurrency;
            OutputCurrency = request.OutputCurrency;
            Amount = request.Amount;
        }

        public decimal Value { get; set; }
    }
}
