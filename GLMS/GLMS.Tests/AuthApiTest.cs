using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using GLMS.Tests.Infrastructure;
using Xunit;

namespace GLMS.Tests;

public class AuthApiTests : IClassFixture<GlmsWebApplicationFactory>
{
    private readonly GlmsWebApplicationFactory _factory;

    public AuthApiTests(GlmsWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsToken()
    {
        // Arrange
        var client = _factory.CreateClient();
        var loginRequest = new
        {
            username = "admin",
            password = "Admin@123"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(content);
        var root = json.RootElement;

        Assert.True(root.TryGetProperty("token", out var tokenProperty));
        var token = tokenProperty.GetString();
        Assert.False(string.IsNullOrEmpty(token));
        Assert.True(token!.Length > 20); // JWT tokens are typically quite long
    }

    [Fact]
    public async Task Login_InvalidCredentials_Returns401()
    {
        // Arrange
        var client = _factory.CreateClient();
        var loginRequest = new
        {
            username = "admin",
            password = "WrongPassword123!"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithoutToken_Returns401()
    {
        // Arrange
        var client = TestAuthHelper.GetUnauthorizedClient(_factory);

        // Act
        var response = await client.GetAsync("/api/contracts");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithValidToken_Returns200()
    {
        // Arrange - Login first to get a real token
        var loginClient = _factory.CreateClient();
        var loginRequest = new
        {
            username = "admin",
            password = "Admin@123"
        };
        var loginResponse = await loginClient.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(loginContent);
        var token = json.RootElement.GetProperty("token").GetString();

        // Arrange - Create authorized client with real token
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.GetAsync("/api/contracts");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}