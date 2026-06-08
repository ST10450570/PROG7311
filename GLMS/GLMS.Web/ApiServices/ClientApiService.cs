using GLMS.Web.ViewModels.Clients;

namespace GLMS.Web.ApiServices
{
    public class ClientApiService : BaseApiService
    {
        public ClientApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, ILogger<ClientApiService> logger, IConfiguration config)
            : base(httpClientFactory, httpContextAccessor, logger, config)
        {
        }

        public async Task<List<ClientViewModel>> GetAllAsync()
        {
            try
            {
                var client = CreateAuthorizedClient();
                var response = await client.GetAsync("/api/clients");
                if (!response.IsSuccessStatusCode) return new List<ClientViewModel>();
                return await response.Content.ReadFromJsonAsync<List<ClientViewModel>>() ?? new List<ClientViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching clients");
                return new List<ClientViewModel>();
            }
        }

        public async Task<ClientViewModel?> GetByIdAsync(int id)
        {
            try
            {
                var client = CreateAuthorizedClient();
                var response = await client.GetAsync($"/api/clients/{id}");
                if (!response.IsSuccessStatusCode) return null;
                return await response.Content.ReadFromJsonAsync<ClientViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching client {id}");
                return null;
            }
        }

        public async Task<bool> CreateAsync(CreateClientViewModel model)
        {
            try
            {
                var client = CreateAuthorizedClient();
                var response = await client.PostAsJsonAsync("/api/clients", model);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating client");
                return false;
            }
        }

        public async Task<bool> UpdateAsync(int id, CreateClientViewModel model)
        {
            try
            {
                var client = CreateAuthorizedClient();
                var response = await client.PutAsJsonAsync($"/api/clients/{id}", model);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating client {id}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var client = CreateAuthorizedClient();
                var response = await client.DeleteAsync($"/api/clients/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting client {id}");
                return false;
            }
        }
    }
}