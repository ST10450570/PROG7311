using System.Net.Http;
using System.Net.Http.Headers;

namespace GLMS.Tests.Infrastructure;

public static class TestAuthHelper
{
    /// <summary>
    /// Creates an HttpClient with a test authorization header pre-configured.
    /// The test auth handler in the factory will validate any request with this scheme.
    /// </summary>
    public static HttpClient GetAuthorizedClient(GlmsWebApplicationFactory factory)
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
        return client;
    }

    /// <summary>
    /// Creates an unauthenticated HttpClient for testing 401 responses.
    /// </summary>
    public static HttpClient GetUnauthorizedClient(GlmsWebApplicationFactory factory)
    {
        var client = factory.CreateClient();
        // Ensure no auth header is present
        client.DefaultRequestHeaders.Authorization = null;
        return client;
    }
}