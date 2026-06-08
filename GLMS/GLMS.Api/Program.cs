using GLMS.Api.Data;
using GLMS.Api.Factories;
using GLMS.Api.Observers;
using GLMS.Api.Repositories;
using GLMS.Api.Repositories.Interfaces;
using GLMS.Api.Services;
using GLMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace GLMS.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Controllers (API only, not MVC)
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler =
                        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.PropertyNamingPolicy =
                        System.Text.Json.JsonNamingPolicy.CamelCase;
                });

            // 2. Swagger with JWT bearer button
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "GLMS API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter: Bearer {your JWT token}"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath)) c.IncludeXmlComments(xmlPath);
            });

            // 3. Database
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("GLMS")));

            // 4. JWT Authentication
            var jwtKey = builder.Configuration["Jwt:Key"]
                ?? throw new Exception("JWT Key not configured");
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
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtKey))
                    };
                });

            // 5. Authorization
            builder.Services.AddAuthorization();

            // 6. CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowWeb", policy =>
                {
                    policy.WithOrigins(
                            "http://localhost:5000",
                            "https://localhost:5001",
                            "http://localhost:7000",
                            "https://localhost:7001",
                            "http://glms-web",
                            "http://glms-web:8080")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            // 7. HttpClient for ExchangeRateService
            builder.Services.AddHttpClient<ExchangeRateService>();

            // 8. Repositories
            builder.Services.AddScoped<IClientRepository, ClientRepository>();
            builder.Services.AddScoped<IContractRepository, ContractRepository>();
            builder.Services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();

            // 9. Services
            builder.Services.AddScoped<IClientService, ClientService>();
            builder.Services.AddScoped<IContractService, ContractService>();
            builder.Services.AddScoped<IServiceRequestService, ServiceRequestService>();
            builder.Services.AddScoped<IExchangeRateService, ExchangeRateService>();
            builder.Services.AddScoped<FileService>();

            // 10. Factory resolver
            builder.Services.AddSingleton<ContractFactoryResolver>();

            // 11. Observer pattern (Singleton with observers attached)
            builder.Services.AddSingleton<ContractSubject>(sp =>
            {
                var subject = new ContractSubject();
                var logPath = Path.Combine(Directory.GetCurrentDirectory(), "audit.log");
                subject.Attach(new AuditLogObserver(logPath));
                subject.Attach(new EmailNotificationObserver("smtp.techmove.local"));
                return subject;
            });

            var app = builder.Build();

            // 12. Swagger (both environments for demo/marking purposes)
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "GLMS API v1");
                c.RoutePrefix = "swagger";
            });

            // 13. HTTPS redirect
            app.UseHttpsRedirection();

            // 14. CORS (must be before Auth)
            app.UseCors("AllowWeb");

            // 15. Authentication
            app.UseAuthentication();

            // 16. Authorization
            app.UseAuthorization();

            // 17. Auto-migrate database on startup
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.Migrate();
            }

            // 18. Map controllers
            app.MapControllers();

            app.Run();
        }
    }
}