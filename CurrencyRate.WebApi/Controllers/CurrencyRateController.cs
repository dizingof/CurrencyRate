using CurrencyRate.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyRate.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyRateController : ControllerBase
    {
        private readonly ICurrencyRateService _currencyRateService;

        public CurrencyRateController(ICurrencyRateService currencyRateService)
        {
            _currencyRateService = currencyRateService;
        }

        [HttpGet("{currencyCode}")]
        public async Task<IActionResult> GetLatestRates(string currencyCode)
        {
            var latestRatesByCode = await _currencyRateService.GetLatestRateByCodeAsync(currencyCode);
            if (latestRatesByCode.Count == 0)
            {
                return NoContent();
            }            
            return Ok(latestRatesByCode);
        }
    }
}