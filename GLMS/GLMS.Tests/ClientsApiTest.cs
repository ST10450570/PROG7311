using GLMS.Api.DTOs.Clients;
using GLMS.Api.Models;
using GLMS.Api.DTOs;
using GLMS.Api.Models;
using GLMS.Tests.Infrastructure;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
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
        // Act
        var response = await _client.GetAsync("/api/clients");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetClients_ReturnsNonEmptyList()
    {
        // Act
        var response = await _client.GetAsync("/api/clients");
        var clients = await response.Content.ReadFromJsonAsync<List<Client>>();

        // Assert
        Assert.NotNull(clients);
        Assert.True(clients!.Count > 0);
    }

    [Fact]
    public async Task GetClientById_ValidId_ReturnsClient()
    {
        // Act
        var response = await _client.GetAsync("/api/clients/1");
        var client = await response.Content.ReadFromJsonAsync<Client>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(client);
        Assert.Equal(1, client!.Id);
        Assert.Equal("Acme Corporation", client.Name);
    }

    [Fact]
    public async Task GetClientById_InvalidId_Returns404()
    {
        // Act
        var response = await _client.GetAsync("/api/clients/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateClient_ValidData_Returns201()
    {
        // Arrange
        var createDto = new CreateClientDto
        {
            Name = "Test Client Inc.",
            ContactEmail = "test@testclient.com",
            ContactPhone = "+1-555-1234",
            Region = "Asia Pacific"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/clients", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.Contains("/api/clients/", response.Headers.Location!.ToString());
    }

    [Fact]
    public async Task CreateClient_ThenGetById_DataMatches()
    {
        // Arrange
        var createDto = new CreateClientDto
        {
            Name = "Data Integrity Corp",
            ContactEmail = "integrity@dicorp.com",
            ContactPhone = "+27-21-555-9876",
            Region = "Africa"
        };

        // Act - Create
        var createResponse = await _client.PostAsJsonAsync("/api/clients", createDto);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        // Act - Get by location
        var getResponse = await _client.GetAsync(createResponse.Headers.Location!.ToString());
        var client = await getResponse.Content.ReadFromJsonAsync<Client>();

        // Assert - Data integrity
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.NotNull(client);
        Assert.Equal(createDto.Name, client!.Name);
        Assert.Equal(createDto.ContactEmail, client.ContactEmail);
        Assert.Equal(createDto.ContactPhone, client.ContactPhone);
        Assert.Equal(createDto.Region, client.Region);
        Assert.True(client.Id > 0);
    }

    [Fact]
    public async Task UpdateClient_ValidData_Returns204()
    {
        // Arrange
        var updateDto = new
        {
            Name = "Updated Client Name",
            ContactEmail = "updated@email.com",
            ContactPhone = "+1-555-9999",
            Region = "South America"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/clients/1", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify the update persisted
        var getResponse = await _client.GetAsync("/api/clients/1");
        var client = await getResponse.Content.ReadFromJsonAsync<Client>();
        Assert.Equal("Updated Client Name", client!.Name);
        Assert.Equal("updated@email.com", client.ContactEmail);
    }

    [Fact]
    public async Task DeleteClient_ValidId_Returns204()
    {
        // First create a client to delete
        var createDto = new CreateClientDto
        {
            Name = "Temp Delete Client",
            ContactEmail = "temp@delete.com",
            ContactPhone = "+1-555-0000",
            Region = "Temporary"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/clients", createDto);
        var location = createResponse.Headers.Location!.ToString();

        // Act
        var response = await _client.DeleteAsync(location);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify it's gone
        var getResponse = await _client.GetAsync(location);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}