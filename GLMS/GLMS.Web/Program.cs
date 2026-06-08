using GLMS.Web.Data;
using GLMS.Web.Factories;
using GLMS.Web.Observers;
using GLMS.Web.Services;
using Microsoft.EntityFrameworkCore;

namespace GLMS.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("GLMS")));

            builder.Services.AddHttpClient<ExchangeRateService>();

            builder.Services.AddScoped<FileService>();
            builder.Services.AddSingleton<ContractFactoryResolver>();

            builder.Services.AddSingleton<ContractSubject>(sp =>
            {
                var subject = new ContractSubject();
                var logPath = Path.Combine(Directory.GetCurrentDirectory(), "audit.log");
                subject.Attach(new AuditLogObserver(logPath));
                subject.Attach(new EmailNotificationObserver("smtp.techmove.local"));
                return subject;
            });

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
