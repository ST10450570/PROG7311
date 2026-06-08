using GLMS.Web.ApiServices;
using GLMS.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// 1. AddControllersWithViews()
builder.Services.AddControllersWithViews();

// 2. AddHttpClient() — for IHttpClientFactory
builder.Services.AddHttpClient();

// 3. AddHttpContextAccessor() — needed to read cookie in ApiServices
builder.Services.AddHttpContextAccessor();

// 4. AddAuthentication with Cookie scheme
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "glms_jwt";
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
    });

// 5. AddAuthorization
builder.Services.AddAuthorization();

// 6. Register all ApiService classes as Scoped
builder.Services.AddScoped<AuthApiService>();
builder.Services.AddScoped<ClientApiService>();
builder.Services.AddScoped<ContractApiService>();
builder.Services.AddScoped<ServiceRequestApiService>();

// 7. Register FileService as Scoped
builder.Services.AddScoped<FileService>();

// 8. Build app
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// 9. UseStaticFiles
app.UseStaticFiles();

// 10. UseRouting
app.UseRouting();

// 11. UseAuthentication (BEFORE UseAuthorization)
app.UseAuthentication();

// 12. UseAuthorization
app.UseAuthorization();

// 13. MapControllerRoute default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// 14. app.Run()
app.Run();