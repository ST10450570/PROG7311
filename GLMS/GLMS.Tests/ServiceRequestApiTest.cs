using GLMS.Api.DTOs.ServiceRequests;
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
        // Act
        var response = await _client.GetAsync("/api/servicerequests");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetServiceRequestById_ValidId_ReturnsRequest()
    {
        // Act
        var response = await _client.GetAsync("/api/servicerequests/1");
        var serviceRequest = await response.Content.ReadFromJsonAsync<ServiceRequest>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(serviceRequest);
        Assert.Equal(1, serviceRequest!.Id);
    }

    [Fact]
    public async Task GetServiceRequestById_InvalidId_Returns404()
    {
        // Act
        var response = await _client.GetAsync("/api/servicerequests/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateServiceRequest_AgainstActiveContract_Returns201()
    {
        // Arrange - Contract 1 is Active in seed data
        var createDto = new CreateServiceRequestDto
        {
            ContractId = 1,
            Description = "Test service request for active contract",
            CostUsd = 500.00m
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/servicerequests", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.Contains("/api/servicerequests/", response.Headers.Location!.ToString());
    }

    [Fact]
    public async Task CreateServiceRequest_AgainstDraftContract_Returns400()
    {
        // Arrange - Contract 2 is Draft in seed data
        var createDto = new CreateServiceRequestDto
        {
            ContractId = 2,
            Description = "Test service request for draft contract",
            CostUsd = 750.00m
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/servicerequests", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateServiceRequest_ThenGet_DataIntegrityTest()
    {
        // Arrange
        var createDto = new CreateServiceRequestDto
        {
            ContractId = 1,
            Description = "Data integrity test service request",
            CostUsd = 1250.00m
        };

        // Act - Create
        var createResponse = await _client.PostAsJsonAsync("/api/servicerequests", createDto);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        // Act - Get by location
        var getResponse = await _client.GetAsync(createResponse.Headers.Location!.ToString());
        var serviceRequest = await getResponse.Content.ReadFromJsonAsync<ServiceRequest>();

        // Assert - Data integrity
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.NotNull(serviceRequest);
        Assert.Equal(createDto.ContractId, serviceRequest!.ContractId);
        Assert.Equal(createDto.Description, serviceRequest.Description);
        Assert.Equal(createDto.CostUsd, serviceRequest.CostUsd);
        Assert.Equal(ServiceRequestStatus.Pending, serviceRequest.Status);
        Assert.True(serviceRequest.Id > 0);
        Assert.True(serviceRequest.CostZar > 0); // Should be auto-calculated
        Assert.True(serviceRequest.CreatedOn.Date == DateTime.UtcNow.Date);
    }

    [Fact]
    public async Task DeleteServiceRequest_ValidId_Returns204()
    {
        // First create a service request to delete
        var createDto = new CreateServiceRequestDto
        {
            ContractId = 1,
            Description = "Temporary service request for deletion",
            CostUsd = 100.00m
        };
        var createResponse = await _client.PostAsJsonAsync("/api/servicerequests", createDto);
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