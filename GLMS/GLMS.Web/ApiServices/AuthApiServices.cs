using GLMS.Web.ViewModels.Auth;

namespace GLMS.Web.ApiServices
{
    public class AuthApiService : BaseApiService
    {
        public AuthApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, ILogger<AuthApiService> logger, IConfiguration config)
            : base(httpClientFactory, httpContextAccessor, logger, config)
        {
        }

        public async Task<string?> LoginAsync(string username, string password)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri(_baseUrl);

                var response = await client.PostAsJsonAsync("/api/auth/login", new { Username = username, Password = password });

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
                    return result?.Token;
                }

                _logger.LogWarning($"Login failed for user {username}. Status: {response.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login API call.");
                return null;
            }
        }
    }
}