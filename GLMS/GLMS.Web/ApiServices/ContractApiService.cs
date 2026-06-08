using GLMS.Web.ViewModels;
using GLMS.Web.ViewModels.Contracts;
using System.Net.Http.Headers;

namespace GLMS.Web.ApiServices
{
    public class ContractApiService : BaseApiService
    {
        public ContractApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, ILogger<ContractApiService> logger, IConfiguration config)
            : base(httpClientFactory, httpContextAccessor, logger, config)
        {
        }

        public async Task<List<ContractViewModel>> GetContractsAsync(ContractStatus? status = null, DateTime? from = null, DateTime? to = null)
        {
            try
            {
                var client = CreateAuthorizedClient();
                var queryParams = new List<string>();

                if (status.HasValue) queryParams.Add($"status={status.Value}");
                if (from.HasValue) queryParams.Add($"startDateFrom={from.Value:yyyy-MM-dd}");
                if (to.HasValue) queryParams.Add($"startDateTo={to.Value:yyyy-MM-dd}");

                var url = "/api/contracts";
                if (queryParams.Any()) url += "?" + string.Join("&", queryParams);

                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode) return new List<ContractViewModel>();
                return await response.Content.ReadFromJsonAsync<List<ContractViewModel>>() ?? new List<ContractViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching contracts");
                return new List<ContractViewModel>();
            }
        }

        public async Task<ContractViewModel?> GetByIdAsync(int id)
        {
            try
            {
                var client = CreateAuthorizedClient();
                var response = await client.GetAsync($"/api/contracts/{id}");
                if (!response.IsSuccessStatusCode) return null;
                return await response.Content.ReadFromJsonAsync<ContractViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching contract {id}");
                return null;
            }
        }

        public async Task<ContractViewModel?> CreateAsync(CreateContractViewModel model)
        {
            try
            {
                var client = CreateAuthorizedClient();
                var response = await client.PostAsJsonAsync("/api/contracts", new
                {
                    model.ClientId,
                    model.StartDate,
                    model.EndDate,
                    model.Status,
                    model.ServiceLevel
                });

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ContractViewModel>();
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating contract");
                return null;
            }
        }

        public async Task<bool> UpdateStatusAsync(int id, ContractStatus status)
        {
            try
            {
                var client = CreateAuthorizedClient();
                var response = await client.PatchAsJsonAsync($"/api/contracts/{id}/status", new { Status = status });
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating status for contract {id}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var client = CreateAuthorizedClient();
                var response = await client.DeleteAsync($"/api/contracts/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting contract {id}");
                return false;
            }
        }

        public async Task<bool> UploadAgreementAsync(int contractId, IFormFile file)
        {
            try
            {
                var client = CreateAuthorizedClient();
                using var content = new MultipartFormDataContent();

                var fileContent = new StreamContent(file.OpenReadStream());
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(file.ContentType);
                content.Add(fileContent, "file", file.FileName);

                var response = await client.PostAsync($"/api/contracts/{contractId}/upload-agreement", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error uploading agreement for contract {contractId}");
                return false;
            }
        }

        public async Task<Stream?> DownloadAgreementAsync(int contractId)
        {
            try
            {
                var client = CreateAuthorizedClient();
                var response = await client.GetAsync($"/api/contracts/{contractId}/download-agreement");
                if (!response.IsSuccessStatusCode) return null;
                return await response.Content.ReadAsStreamAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error downloading agreement for contract {contractId}");
                return null;
            }
        }
    }
}