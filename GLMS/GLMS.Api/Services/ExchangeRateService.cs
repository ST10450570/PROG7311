using GLMS.Api.Services.Interfaces;
using System.Text.Json;

namespace GLMS.Api.Services
{
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public ExchangeRateService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<decimal> GetUsdToZarRateAsync()
        {
            var baseUrl = _config["ExchangeRate:BaseUrl"];
            if (string.IsNullOrEmpty(baseUrl)) throw new Exception("Exchange rate URL not configured.");

            var response = await _httpClient.GetAsync(baseUrl);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(content);

            if (jsonDoc.RootElement.TryGetProperty("rates", out var rates) &&
                rates.TryGetProperty("ZAR", out var zarRate))
            {
                return zarRate.GetDecimal();
            }

            throw new Exception("Unable to fetch ZAR exchange rate from provider.");
        }
    }
}
