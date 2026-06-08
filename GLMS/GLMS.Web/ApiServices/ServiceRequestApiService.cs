using GLMS.Web.ViewModels.ServiceRequests;
using System.Text.Json;

namespace GLMS.Web.ApiServices
{
    public class ServiceRequestApiService : BaseApiService
    {
        public ServiceRequestApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, ILogger<ServiceRequestApiService> logger, IConfiguration config)
            : base(httpClientFactory, httpContextAccessor, logger, config)
        {
        }

        public async Task<List<ServiceRequestViewModel>> GetAllAsync()
        {
            try
            {
                var client = CreateAuthorizedClient();
                var response = await client.GetAsync("/api/servicerequests");
                if (!response.IsSuccessStatusCode) return new List<ServiceRequestViewModel>();
                return await response.Content.ReadFromJsonAsync<List<ServiceRequestViewModel>>() ?? new List<ServiceRequestViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching service requests");
                return new List<ServiceRequestViewModel>();
            }
        }

        public async Task<ServiceRequestViewModel?> GetByIdAsync(int id)
        {
            try
            {
                var client = CreateAuthorizedClient();
                var response = await client.GetAsync($"/api/servicerequests/{id}");
                if (!response.IsSuccessStatusCode) return null;
                return await response.Content.ReadFromJsonAsync<ServiceRequestViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching service request {id}");
                return null;
            }
        }

        public async Task<bool> CreateAsync(CreateServiceRequestViewModel model)
        {
            try
            {
                var client = CreateAuthorizedClient();
                var response = await client.PostAsJsonAsync("/api/servicerequests", new
                {
                    model.ContractId,
                    model.Description,
                    model.CostUsd
                });
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating service request");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var client = CreateAuthorizedClient();
                var response = await client.DeleteAsync($"/api/servicerequests/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting service request {id}");
                return false;
            }
        }

        public async Task<decimal> GetLiveExchangeRateAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri(_baseUrl);
                var response = await client.GetAsync("/api/exchangerate/usd-to-zar");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var jsonDoc = JsonDocument.Parse(content);
                    if (jsonDoc.RootElement.TryGetProperty("rate", out var rateElement))
                    {
                        return rateElement.GetDecimal();
                    }
                }
                return 0m;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching exchange rate from API.");
                return 0m;
            }
        }
    }
}