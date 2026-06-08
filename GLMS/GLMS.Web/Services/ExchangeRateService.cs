using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace GLMS.Web.Services
{
    public class ExchangeRateService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ExchangeRateService> _logger;

        // Added ILogger to help track down any future API errors. I used Ai in this section as my own previous code was not using the api to fetch data
        // ASP.NET Core will inject this automatically.
        public ExchangeRateService(HttpClient httpClient, ILogger<ExchangeRateService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<decimal> GetUsdToZarRateAsync()
        {
            // Using a reliable public API that supports USD base for free.
            var url = "https://api.exchangerate-api.com/v4/latest/USD";

            try
            {
                var response = await _httpClient.GetAsync(url);

                // If the API returns a bad status code, log the exact error to the console
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Exchange Rate API failed with status {StatusCode}. Details: {Error}", response.StatusCode, errorContent);

                    return 18.50m; // Fallback
                }

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);

                var rate = doc.RootElement
                    .GetProperty("rates")
                    .GetProperty("ZAR")
                    .GetDecimal();

                return rate;
            }
            catch (Exception ex)
            {
                // Log any JSON parsing or network connection issues
                _logger.LogError(ex, "An exception occurred while fetching the exchange rate.");
                return 18.50m;
            }
        }
    }
}