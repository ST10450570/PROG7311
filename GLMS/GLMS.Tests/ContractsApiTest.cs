using GLMS.Api.DTOs.Contracts;
using GLMS.Api.Models;
using GLMS.Tests.Infrastructure;
using System.Net;
using System.Net.Http.Json;
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
        var response = await _client.GetAsync("/api/contracts");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetContracts_ReturnsListOfContracts()
    {
        var response = await _client.GetAsync("/api/contracts");
        var contracts = await response.Content.ReadFromJsonAsync<List<ContractDto>>();
        Assert.NotNull(contracts);
        Assert.True(contracts!.Count > 0);
    }

    [Fact]
    public async Task GetContractById_ValidId_ReturnsContract()
    {
        var response = await _client.GetAsync("/api/contracts/1");
        var contract = await response.Content.ReadFromJsonAsync<ContractDto>();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(contract);
        Assert.Equal(1, contract!.Id);
    }

    [Fact]
    public async Task GetContractById_InvalidId_Returns404()
    {
        var response = await _client.GetAsync("/api/contracts/99999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateContract_ValidData_Returns201()
    {
        var createDto = new CreateContractDto
        {
            ClientId = 1,
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2026, 12, 31),
            Status = ContractStatus.Draft,
            ServiceLevel = ServiceLevel.Premium
        };
        var response = await _client.PostAsJsonAsync("/api/contracts", createDto);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
    }

    [Fact]
    public async Task CreateContract_ThenGetById_ReturnsCreatedContract()
    {
        var createDto = new CreateContractDto
        {
            ClientId = 2,
            StartDate = new DateTime(2025, 3, 1),
            EndDate = new DateTime(2026, 2, 28),
            Status = ContractStatus.Draft,
            ServiceLevel = ServiceLevel.Standard
        };
        var createResponse = await _client.PostAsJsonAsync("/api/contracts", createDto);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var getResponse = await _client.GetAsync(createResponse.Headers.Location!.ToString());
        var contract = await getResponse.Content.ReadFromJsonAsync<ContractDto>();

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.NotNull(contract);
        Assert.Equal(createDto.ClientId, contract!.ClientId);
        Assert.Equal(createDto.StartDate, contract.StartDate);
        Assert.Equal(createDto.EndDate, contract.EndDate);
        Assert.Equal("Draft", contract.Status);
        Assert.Equal("Standard", contract.ServiceLevel);
    }

    [Fact]
    public async Task UpdateContractStatus_ValidStatus_Returns204()
    {
        var statusUpdate = new UpdateContractStatusDto { Status = ContractStatus.Active };
        var response = await _client.PatchAsJsonAsync("/api/contracts/1/status", statusUpdate);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync("/api/contracts/1");
        var contract = await getResponse.Content.ReadFromJsonAsync<ContractDto>();
        Assert.Equal("Active", contract!.Status);
    }

    [Fact]
    public async Task GetContracts_FilterByStatus_ReturnsOnlyMatchingContracts()
    {
        var response = await _client.GetAsync("/api/contracts?status=Active");
        var contracts = await response.Content.ReadFromJsonAsync<List<ContractDto>>();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(contracts);
        Assert.All(contracts!, c => Assert.Equal("Active", c.Status));
    }

    [Fact]
    public async Task DeleteContract_ValidId_Returns204()
    {
        var createDto = new CreateContractDto
        {
            ClientId = 1,
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2026, 12, 31),
            Status = ContractStatus.Draft,
            ServiceLevel = ServiceLevel.Standard
        };
        var createResponse = await _client.PostAsJsonAsync("/api/contracts", createDto);
        var location = createResponse.Headers.Location!.ToString();

        var response = await _client.DeleteAsync(location);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync(location);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteContract_InvalidId_Returns404()
    {
        var response = await _client.DeleteAsync("/api/contracts/99999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}