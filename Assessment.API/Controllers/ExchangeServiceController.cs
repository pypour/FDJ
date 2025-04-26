using Assessment.Contract.Interfaces.Currency;
using Assessment.Contract.Models;
using Microsoft.AspNetCore.Mvc;

namespace Assessment.API.Controllers
{
    public class ExchangeServiceController(ICurrencyService currencyService) : BaseController
    {
        //We should return ApiResult<ExchangeResult> to have common structure for all APIs
        [HttpPost]
        [ProducesResponseType(typeof(ExchangeResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Convert(ExchangeRequest request, CancellationToken cancellationToken)
        {
            var result = await currencyService.ConvertAsync(request, cancellationToken);
            return Ok(result);
        }
    }
}
