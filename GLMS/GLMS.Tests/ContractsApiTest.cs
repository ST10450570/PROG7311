using GLMS.Api.DTOs.Contracts;
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

public class ContractsApiTests : IClassFixture<GlmsWebApplicationFactory>
{
    private readonly GlmsWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public ContractsApiTests(GlmsWebApplicationFactory factory)
    {
        _factory = factory;
        _client = TestAuthHelper.GetAuthorizedClient(_factory);
    }

    [Fact]
    public async Task GetContracts_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/api/contracts");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetContracts_ReturnsListOfContracts()
    {
        // Act
        var response = await _client.GetAsync("/api/contracts");
        var contracts = await response.Content.ReadFromJsonAsync<List<Contract>>();

        // Assert
        Assert.NotNull(contracts);
        Assert.True(contracts!.Count > 0);
    }

    [Fact]
    public async Task GetContractById_ValidId_ReturnsContract()
    {
        // Act
        var response = await _client.GetAsync("/api/contracts/1");
        var contract = await response.Content.ReadFromJsonAsync<Contract>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(contract);
        Assert.Equal(1, contract!.Id);
    }

    [Fact]
    public async Task GetContractById_InvalidId_Returns404()
    {
        // Act
        var response = await _client.GetAsync("/api/contracts/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateContract_ValidData_Returns201()
    {
        // Arrange
        var createDto = new CreateContractDto
        {
            ClientId = 1,
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2026, 12, 31),
            Status = ContractStatus.Draft,
            ServiceLevel = ServiceLevel.Premium,
            SignedAgreementPath = "/documents/contracts/new_agreement.pdf"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/contracts", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.Contains("/api/contracts/", response.Headers.Location!.ToString());
    }

    [Fact]
    public async Task CreateContract_ThenGetById_ReturnsCreatedContract()
    {
        // Arrange
        var createDto = new CreateContractDto
        {
            ClientId = 2,
            StartDate = new DateTime(2025, 3, 1),
            EndDate = new DateTime(2026, 2, 28),
            Status = ContractStatus.Draft,
            ServiceLevel = ServiceLevel.Standard,
            SignedAgreementPath = "/documents/contracts/integrity_test.pdf"
        };

        // Act - Create
        var createResponse = await _client.PostAsJsonAsync("/api/contracts", createDto);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        // Act - Get by location
        var getResponse = await _client.GetAsync(createResponse.Headers.Location!.ToString());
        var contract = await getResponse.Content.ReadFromJsonAsync<Contract>();

        // Assert - Data integrity
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.NotNull(contract);
        Assert.Equal(createDto.ClientId, contract!.ClientId);
        Assert.Equal(createDto.StartDate, contract.StartDate);
        Assert.Equal(createDto.EndDate, contract.EndDate);
        Assert.Equal(createDto.Status, contract.Status);
        Assert.Equal(createDto.ServiceLevel, contract.ServiceLevel);
        Assert.Equal(createDto.SignedAgreementPath, contract.SignedAgreementPath);
    }

    [Fact]
    public async Task UpdateContractStatus_ValidStatus_Returns204()
    {
        // Arrange
        var statusUpdate = new { status = 1 }; // Active

        // Act
        var response = await _client.PatchAsJsonAsync("/api/contracts/1/status", statusUpdate);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify the update actually persisted
        var getResponse = await _client.GetAsync("/api/contracts/1");
        var contract = await getResponse.Content.ReadFromJsonAsync<Contract>();
        Assert.Equal(ContractStatus.Active, contract!.Status);
    }

    [Fact]
    public async Task GetContracts_FilterByStatus_ReturnsOnlyMatchingContracts()
    {
        // Act
        var response = await _client.GetAsync("/api/contracts?status=Active");
        var contracts = await response.Content.ReadFromJsonAsync<List<Contract>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(contracts);
        Assert.True(contracts!.Count > 0);
        Assert.All(contracts, c => Assert.Equal(ContractStatus.Active, c.Status));
    }

    [Fact]
    public async Task DeleteContract_ValidId_Returns204()
    {
        // First create a contract to delete (to avoid affecting seeded data used by other tests)
        var createDto = new CreateContractDto
        {
            ClientId = 1,
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2026, 12, 31),
            Status = ContractStatus.Draft,
            ServiceLevel = ServiceLevel.Standard,
            SignedAgreementPath = "/temp.pdf"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/contracts", createDto);
        var location = createResponse.Headers.Location!.ToString();

        // Act
        var response = await _client.DeleteAsync(location);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify it's gone
        var getResponse = await _client.GetAsync(location);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteContract_InvalidId_Returns404()
    {
        // Act
        var response = await _client.DeleteAsync("/api/contracts/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}