using GLMS.Api.Data;
using GLMS.Api.Observers;
using GLMS.Api.Repositories;
using GLMS.Api.Repositories.Interfaces;
using GLMS.Api.Services;
using GLMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

namespace GLMS.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. AddControllers with JSON options
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            });

            // 2. AddEndpointsApiExplorer
            builder.Services.AddEndpointsApiExplorer();

            // 3. AddOpenApi (for .NET 10)
            builder.Services.AddOpenApi();

            // 4. AddDbContext
            var connectionString = Environment.GetEnvironmentVariable("GLMS_DOCKER_CONN")
                                   ?? builder.Configuration.GetConnectionString("GLMS");

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            // 5. AddAuthentication
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                    };
                });

            // 6. AddAuthorization
            builder.Services.AddAuthorization();

            // 7. AddCors
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("GLMSCorsPolicy", policy =>
                {
                    policy.WithOrigins(
                            "http://localhost:5000",
                            "https://localhost:5001",
                            "http://localhost:7000",
                            "https://localhost:7001",
                            "http://glms-web")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            // 8. AddHttpClient
            builder.Services.AddHttpClient<IExchangeRateService, ExchangeRateService>();

            // 9. Register Repositories and Services
            builder.Services.AddScoped<IClientRepository, ClientRepository>();
            builder.Services.AddScoped<IContractRepository, ContractRepository>();
            builder.Services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();

            builder.Services.AddScoped<IClientService, ClientService>();
            builder.Services.AddScoped<IContractService, ContractService>();
            builder.Services.AddScoped<IServiceRequestService, ServiceRequestService>();
            builder.Services.AddScoped<IExchangeRateService, ExchangeRateService>();

            // 10. Register ContractSubject as Singleton and attach observers
            var contractSubject = new ContractSubject();
            contractSubject.Attach(new AuditLogObserver(Path.Combine(Directory.GetCurrentDirectory(), "audit.log")));
            contractSubject.Attach(new EmailNotificationObserver("smtp.techmove.local"));
            builder.Services.AddSingleton(contractSubject);

            var app = builder.Build();

            // 11. MapOpenApi (for .NET 10)
            app.MapOpenApi();

            // 12. UseHttpsRedirection
            app.UseHttpsRedirection();

            // 13. UseCors
            app.UseCors("GLMSCorsPolicy");

            // 14. UseAuthentication
            app.UseAuthentication();

            // 15. UseAuthorization
            app.UseAuthorization();

            // 16. Auto-migrate database on startup
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.Migrate();
            }

            // 17. MapControllers
            app.MapControllers();

            // 18. Run app
            app.Run();
        }
    }
}