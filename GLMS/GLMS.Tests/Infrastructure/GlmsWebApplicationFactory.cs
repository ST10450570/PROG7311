using GLMS.Api;
using GLMS.Api.Data;
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
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null) services.Remove(descriptor);

                services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid()));

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

            var client1 = new GLMS.Api.Models.Client { Id = 1, Name = "Acme Corporation", ContactEmail = "acme@acme.com", ContactPhone = "+1-555-0001", Region = "North America" };
            var client2 = new GLMS.Api.Models.Client { Id = 2, Name = "Beta Corp", ContactEmail = "beta@beta.com", ContactPhone = "+1-555-0002", Region = "Europe" };
            db.Clients.AddRange(client1, client2);

            var contract1 = new GLMS.Api.Models.Contract { Id = 1, ClientId = 1, StartDate = new DateTime(2025, 1, 1), EndDate = new DateTime(2026, 12, 31), Status = GLMS.Api.Models.ContractStatus.Active, ServiceLevel = GLMS.Api.Models.ServiceLevel.Standard };
            var contract2 = new GLMS.Api.Models.Contract { Id = 2, ClientId = 2, StartDate = new DateTime(2025, 3, 1), EndDate = new DateTime(2026, 2, 28), Status = GLMS.Api.Models.ContractStatus.Draft, ServiceLevel = GLMS.Api.Models.ServiceLevel.Premium };
            db.Contracts.AddRange(contract1, contract2);

            var sr1 = new GLMS.Api.Models.ServiceRequest { Id = 1, ContractId = 1, Description = "Initial setup", CostUsd = 100, CostZar = 1900, Status = GLMS.Api.Models.ServiceRequestStatus.Pending, CreatedOn = DateTime.UtcNow };
            db.ServiceRequests.Add(sr1);

            db.SaveChanges();
        }
    }
}