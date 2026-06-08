using System.Net.Http.Headers;

namespace GLMS.Web.ApiServices
{
    public interface IApiService
    {
        HttpClient CreateAuthorizedClient();
    }

    public abstract class BaseApiService : IApiService
    {
        protected readonly IHttpClientFactory _httpClientFactory;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly ILogger _logger;
        protected readonly string _baseUrl;

        protected BaseApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, ILogger logger, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _baseUrl = config["ApiSettings:BaseUrl"] ?? "https://localhost:7100";
        }

        public HttpClient CreateAuthorizedClient()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_baseUrl);

            var token = _httpContextAccessor.HttpContext?.User.FindFirst("jwt")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }
    }
}