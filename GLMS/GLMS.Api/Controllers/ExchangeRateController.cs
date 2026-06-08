using GLMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GLMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExchangeRateController : ControllerBase
    {
        private readonly IExchangeRateService _exchangeRateService;

        public ExchangeRateController(IExchangeRateService exchangeRateService)
        {
            _exchangeRateService = exchangeRateService;
        }

        [HttpGet("usd-to-zar")]
        public async Task<IActionResult> GetUsdToZar()
        {
            try
            {
                var rate = await _exchangeRateService.GetUsdToZarRateAsync();
                return Ok(new
                {
                    Currency = "ZAR",
                    Rate = rate,
                    Base = "USD",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Failed to retrieve exchange rate",
                    Details = ex.Message
                });
            }
        }
    }
}