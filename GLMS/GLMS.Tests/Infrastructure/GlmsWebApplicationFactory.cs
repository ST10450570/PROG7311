using GLMS.Api.Models;
using GLMS.Api.Data;
using GLMS.Api.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Net.Http;

namespace GLMS.Tests.Infrastructure;

public class GlmsWebApplicationFactory : WebApplicationFactory<GLMS.Api.Program>
{
    private readonly string _databaseName;

    public GlmsWebApplicationFactory()
    {
        _databaseName = Guid.NewGuid().ToString();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // Remove the existing DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add InMemory database
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
            });

            // Bypass JWT authentication for integration tests
            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });

            // Build service provider and seed data
            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<ApplicationDbContext>();

            db.Database.EnsureCreated();

            // Seed test data
            SeedTestData(db);
        });

        builder.UseEnvironment("Testing");
    }

    private void SeedTestData(ApplicationDbContext db)
    {
        if (db.Clients.Any())
        {
            return;
        }

        var client1 = new Client
        {
            Id = 1,
            Name = "Acme Corporation",
            ContactEmail = "contact@acme.com",
            ContactPhone = "+1-555-0100",
            Region = "North America"
        };

        var client2 = new Client
        {
            Id = 2,
            Name = "GlobalTech Solutions",
            ContactEmail = "info@globaltech.com",
            ContactPhone = "+44-20-7946-0958",
            Region = "Europe"
        };

        db.Clients.AddRange(client1, client2);

        var contract1 = new Contract
        {
            Id = 1,
            ClientId = 1,
            StartDate = new DateTime(2024, 1, 1),
            EndDate = new DateTime(2025, 12, 31),
            Status = ContractStatus.Active,
            ServiceLevel = ServiceLevel.Premium,
            SignedAgreementPath = "/documents/contracts/acme_agreement.pdf"
        };

        var contract2 = new Contract
        {
            Id = 2,
            ClientId = 2,
            StartDate = new DateTime(2024, 6, 1),
            EndDate = new DateTime(2025, 5, 31),
            Status = ContractStatus.Draft,
            ServiceLevel = ServiceLevel.Standard,
            SignedAgreementPath = "/documents/contracts/globaltech_agreement.pdf"
        };

        db.Contracts.AddRange(contract1, contract2);

        var serviceRequest1 = new ServiceRequest
        {
            Id = 1,
            ContractId = 1,
            Description = "Initial setup and configuration service request",
            CostUsd = 1500.00m,
            CostZar = 28500.00m,
            Status = ServiceRequestStatus.Pending,
            CreatedOn = new DateTime(2024, 2, 15)
        };

        db.ServiceRequests.AddRange(serviceRequest1);

        db.SaveChanges();
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        return base.CreateHost(builder);
    }
}