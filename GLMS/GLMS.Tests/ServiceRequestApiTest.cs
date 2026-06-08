using GLMS.Api.DTOs.ServiceRequests;
using GLMS.Tests.Infrastructure;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace GLMS.Tests;

public class ServiceRequestsApiTests : IClassFixture<GlmsWebApplicationFactory>
{
    private readonly GlmsWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public ServiceRequestsApiTests(GlmsWebApplicationFactory factory)
    {
        _factory = factory;
        _client = TestAuthHelper.GetAuthorizedClient(_factory);
    }

    [Fact]
    public async Task GetServiceRequests_ReturnsSuccessStatusCode()
    {
        var response = await _client.GetAsync("/api/servicerequests");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetServiceRequestById_ValidId_ReturnsRequest()
    {
        var response = await _client.GetAsync("/api/servicerequests/1");
        var sr = await response.Content.ReadFromJsonAsync<ServiceRequestDto>();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(sr);
        Assert.Equal(1, sr!.Id);
    }

    [Fact]
    public async Task GetServiceRequestById_InvalidId_Returns404()
    {
        var response = await _client.GetAsync("/api/servicerequests/99999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateServiceRequest_AgainstActiveContract_Returns201()
    {
        var createDto = new CreateServiceRequestDto
        {
            ContractId = 1,
            Description = "Test service request for active contract",
            CostUsd = 500.00m
        };
        var response = await _client.PostAsJsonAsync("/api/servicerequests", createDto);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
    }

    [Fact]
    public async Task CreateServiceRequest_AgainstDraftContract_Returns400()
    {
        var createDto = new CreateServiceRequestDto
        {
            ContractId = 2,
            Description = "Test service request for draft contract",
            CostUsd = 750.00m
        };
        var response = await _client.PostAsJsonAsync("/api/servicerequests", createDto);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateServiceRequest_ThenGet_DataIntegrityTest()
    {
        var createDto = new CreateServiceRequestDto
        {
            ContractId = 1,
            Description = "Data integrity test service request",
            CostUsd = 1250.00m
        };
        var createResponse = await _client.PostAsJsonAsync("/api/servicerequests", createDto);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var getResponse = await _client.GetAsync(createResponse.Headers.Location!.ToString());
        var sr = await getResponse.Content.ReadFromJsonAsync<ServiceRequestDto>();

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.NotNull(sr);
        Assert.Equal(createDto.ContractId, sr!.ContractId);
        Assert.Equal(createDto.Description, sr.Description);
        Assert.Equal(createDto.CostUsd, sr.CostUsd);
        Assert.Equal("Pending", sr.Status);
        Assert.True(sr.Id > 0);
        // CostZar may be 0 in tests because ExchangeRateService calls external API
        // which is unavailable in test environment — this is acceptable
    }

    [Fact]
    public async Task DeleteServiceRequest_ValidId_Returns204()
    {
        var createDto = new CreateServiceRequestDto
        {
            ContractId = 1,
            Description = "Temporary SR for deletion",
            CostUsd = 100.00m
        };
        var createResponse = await _client.PostAsJsonAsync("/api/servicerequests", createDto);
        var location = createResponse.Headers.Location!.ToString();

        var response = await _client.DeleteAsync(location);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync(location);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}