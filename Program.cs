using Microsoft.AspNetCore.Authentication.Cookies;
using MSFEP.Components;
using MSFEP.Components.Miscellaneous;
using MSFEP.DataAccess;
using MSFEP.DataAccess.Interfaces;
using MSFEP.Database;
using MSFEP.Database.Interfaces;
using MSFEP.Models;
using MSFEP.Services;
using MSFEP.Services.Interfaces;

namespace MSFEP;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddMemoryCache();
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession();

        builder.Services.AddHttpClient();
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = "custom_auth_token";
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.LoginPath = "/login";
                options.LogoutPath = "/logout";
            });

        builder.Services.AddAuthorizationBuilder()
            .AddPolicy("AdminOnly", policy =>
                policy.RequireClaim("jobTitle", "Admin"))
            .AddPolicy("IsProjectManager", policy =>
                policy.RequireClaim("jobTitle", "Projektmanager"));

        builder.Services.AddCascadingAuthenticationState();

        builder.Services.Configure<GitHubAppConfig>(builder.Configuration.GetSection("GitHubApp"));

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddSingleton<IDbConnectionFactory>(_ => new DbConnectionFactory(connectionString));
        builder.Services.AddScoped<ITimeTrackDataAccess, TimeTrackDataAccess>();
        builder.Services.AddScoped<IProjectDataAccess, ProjectDataAccess>();
        builder.Services.AddScoped<IProjectAffiliationRequestDataAccess, ProjectAffiliationRequestDataAccess>();
        builder.Services.AddScoped<IVerifiedProjectAccessDataAccess, VerifiedProjectAccessDataAccess>();
        builder.Services.AddScoped<IVerificationService, VerificationService>();
        builder.Services.AddScoped<IGitHubAppService, GitHubAppService>();
        builder.Services.AddScoped<IIssuanceService, IssuanceService>();
        builder.Services.AddScoped<SidebarStateService>();

        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        builder.Services.AddControllers();
        builder.Services.AddRazorComponents().AddInteractiveServerComponents();

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAntiforgery();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

        app.Run();
    }
}
