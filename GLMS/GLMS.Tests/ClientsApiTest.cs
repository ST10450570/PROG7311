using GLMS.Api.DTOs.Clients;
using GLMS.Tests.Infrastructure;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace GLMS.Tests;

public class ClientsApiTests : IClassFixture<GlmsWebApplicationFactory>
{
    private readonly GlmsWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public ClientsApiTests(GlmsWebApplicationFactory factory)
    {
        _factory = factory;
        _client = TestAuthHelper.GetAuthorizedClient(_factory);
    }

    [Fact]
    public async Task GetClients_ReturnsSuccessStatusCode()
    {
        var response = await _client.GetAsync("/api/clients");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetClients_ReturnsNonEmptyList()
    {
        var response = await _client.GetAsync("/api/clients");
        var clients = await response.Content.ReadFromJsonAsync<List<ClientDto>>();
        Assert.NotNull(clients);
        Assert.True(clients!.Count > 0);
    }

    [Fact]
    public async Task GetClientById_ValidId_ReturnsClient()
    {
        var response = await _client.GetAsync("/api/clients/1");
        var client = await response.Content.ReadFromJsonAsync<ClientDto>();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(client);
        Assert.Equal(1, client!.Id);
        Assert.Equal("Acme Corporation", client.Name);
    }

    [Fact]
    public async Task GetClientById_InvalidId_Returns404()
    {
        var response = await _client.GetAsync("/api/clients/99999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateClient_ValidData_Returns201()
    {
        var createDto = new CreateClientDto
        {
            Name = "Test Client Inc.",
            ContactEmail = "test@testclient.com",
            ContactPhone = "+1-555-1234",
            Region = "Asia Pacific"
        };
        var response = await _client.PostAsJsonAsync("/api/clients", createDto);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
    }

    [Fact]
    public async Task CreateClient_ThenGetById_DataMatches()
    {
        var createDto = new CreateClientDto
        {
            Name = "Data Integrity Corp",
            ContactEmail = "integrity@dicorp.com",
            ContactPhone = "+27-21-555-9876",
            Region = "Africa"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/clients", createDto);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var getResponse = await _client.GetAsync(createResponse.Headers.Location!.ToString());
        var client = await getResponse.Content.ReadFromJsonAsync<ClientDto>();

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.NotNull(client);
        Assert.Equal(createDto.Name, client!.Name);
        Assert.Equal(createDto.ContactEmail, client.ContactEmail);
        Assert.Equal(createDto.Region, client.Region);
    }

    [Fact]
    public async Task UpdateClient_ValidData_Returns204()
    {
        var updateDto = new CreateClientDto
        {
            Name = "Updated Client Name",
            ContactEmail = "updated@email.com",
            ContactPhone = "+1-555-9999",
            Region = "South America"
        };
        var response = await _client.PutAsJsonAsync("/api/clients/1", updateDto);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteClient_ValidId_Returns204()
    {
        var createDto = new CreateClientDto
        {
            Name = "Temp Delete Client",
            ContactEmail = "temp@delete.com",
            ContactPhone = "+1-555-0000",
            Region = "Temporary"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/clients", createDto);
        var location = createResponse.Headers.Location!.ToString();

        var response = await _client.DeleteAsync(location);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync(location);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}