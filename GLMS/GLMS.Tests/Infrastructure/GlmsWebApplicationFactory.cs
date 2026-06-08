using GLMS.Api;
using GLMS.Api.Data;
using GLMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GLMS.Tests.Infrastructure
{
    public class GlmsWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Replace SQL Server with InMemory
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null) services.Remove(descriptor);

                services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid()));

                // Replace live ExchangeRateService with a stub
                var exchangeDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IExchangeRateService));
                if (exchangeDescriptor != null) services.Remove(exchangeDescriptor);
                services.AddScoped<IExchangeRateService, StubExchangeRateService>();

                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureCreated();
                SeedTestData(db);
            });
        }

        private static void SeedTestData(AppDbContext db)
        {
            if (db.Clients.Any()) return;

            db.Clients.AddRange(
                new GLMS.Api.Models.Client
                {
                    Id = 1,
                    Name = "Acme Corporation",
                    ContactEmail = "acme@acme.com",
                    ContactPhone = "+1-555-0001",
                    Region = "North America"
                },
                new GLMS.Api.Models.Client
                {
                    Id = 2,
                    Name = "Beta Corp",
                    ContactEmail = "beta@beta.com",
                    ContactPhone = "+1-555-0002",
                    Region = "Europe"
                }
            );

            db.Contracts.AddRange(
                new GLMS.Api.Models.Contract
                {
                    Id = 1,
                    ClientId = 1,
                    StartDate = new DateTime(2025, 1, 1),
                    EndDate = new DateTime(2026, 12, 31),
                    Status = GLMS.Api.Models.ContractStatus.Active,
                    ServiceLevel = GLMS.Api.Models.ServiceLevel.Standard
                },
                new GLMS.Api.Models.Contract
                {
                    Id = 2,
                    ClientId = 2,
                    StartDate = new DateTime(2025, 3, 1),
                    EndDate = new DateTime(2026, 2, 28),
                    Status = GLMS.Api.Models.ContractStatus.Draft,
                    ServiceLevel = GLMS.Api.Models.ServiceLevel.Premium
                }
            );

            db.ServiceRequests.Add(
                new GLMS.Api.Models.ServiceRequest
                {
                    Id = 1,
                    ContractId = 1,
                    Description = "Initial setup",
                    CostUsd = 100,
                    CostZar = 1900,
                    Status = GLMS.Api.Models.ServiceRequestStatus.Pending,
                    CreatedOn = DateTime.UtcNow
                }
            );

            db.SaveChanges();
        }
    }

    // Stub replaces live HTTP call in tests
    public class StubExchangeRateService : IExchangeRateService
    {
        public Task<decimal> GetUsdToZarRateAsync() => Task.FromResult(19.00m);
    }
}