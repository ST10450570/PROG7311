using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace GLMS.Tests.Infrastructure
{
    public static class TestAuthHelper
    {
        public static HttpClient GetAuthorizedClient(GlmsWebApplicationFactory factory)
        {
            var client = factory.CreateClient();
            var loginResponse = client.PostAsJsonAsync("/api/auth/login", new { username = "admin", password = "Admin@123" }).Result;
            var content = loginResponse.Content.ReadAsStringAsync().Result;
            var json = JsonDocument.Parse(content);
            var token = json.RootElement.GetProperty("token").GetString();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        public static HttpClient GetUnauthorizedClient(GlmsWebApplicationFactory factory)
        {
            return factory.CreateClient();
        }
    }
}