using ClassifiedAds.Blazor.Modules.AuditLogs.Services;
using ClassifiedAds.Blazor.Modules.Core.Services;
using ClassifiedAds.Blazor.Modules.Files.Services;
using ClassifiedAds.Blazor.Modules.Products.Services;
using ClassifiedAds.Blazor.Modules.Settings.Services;
using ClassifiedAds.Blazor.Modules.Users.Services;
using ClassifiedAds.BlazorServerSide.ConfigurationOptions;
using ClassifiedAds.BlazorServerSide.Services;
using ClassifiedAds.Infrastructure.HealthChecks;
using ClassifiedAds.Infrastructure.HostedServices;
using ClassifiedAds.Infrastructure.HttpMessageHandlers;
using ClassifiedAds.Infrastructure.Logging;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using System;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

var appSettings = new AppSettings();
configuration.Bind(appSettings);

builder.WebHost.UseClassifiedAdsLogger(configuration =>
{
    var appSettings = new AppSettings();
    configuration.Bind(appSettings);
    return appSettings.Logging;
});

// Add services to the container.

services.Configure<AppSettings>(configuration);
services.AddSingleton(appSettings);

if (appSettings.CookiePolicyOptions?.IsEnabled ?? false)
{
    services.Configure<Microsoft.AspNetCore.Builder.CookiePolicyOptions>(options =>
    {
        // This lambda determines whether user consent for non-essential cookies is needed for a given request.
        options.CheckConsentNeeded = context => true;
        options.MinimumSameSitePolicy = appSettings.CookiePolicyOptions.MinimumSameSitePolicy;
        options.Secure = appSettings.CookiePolicyOptions.Secure;
    });
}

services.AddRazorPages();
services.AddServerSideBlazor();

if (appSettings.Azure?.SignalR?.IsEnabled ?? false)
{
    services.AddSignalR()
            .AddAzureSignalR();
}

services.AddScoped<ITokenManager, TokenManager>();
services.AddScoped<TokenProvider>();
services.AddTransient<DebuggingHandler>();

services.AddHttpClient<FileService, FileService>(client =>
{
    client.BaseAddress = new Uri(appSettings.ResourceServer.Endpoint);
    client.Timeout = new TimeSpan(0, 0, 30);
    client.DefaultRequestHeaders.Clear();
}).AddHttpMessageHandler<DebuggingHandler>();

services.AddHttpClient<ConfigurationEntryService, ConfigurationEntryService>(client =>
{
    client.BaseAddress = new Uri(appSettings.ResourceServer.Endpoint);
    client.Timeout = new TimeSpan(0, 0, 30);
    client.DefaultRequestHeaders.Clear();
});
services.AddHttpClient<ProductService, ProductService>(client =>
{
    client.BaseAddress = new Uri(appSettings.ResourceServer.Endpoint);
    client.Timeout = new TimeSpan(0, 0, 30);
    client.DefaultRequestHeaders.Clear();
});
services.AddHttpClient<UserService, UserService>(client =>
{
    client.BaseAddress = new Uri(appSettings.ResourceServer.Endpoint);
    client.Timeout = new TimeSpan(0, 0, 30);
    client.DefaultRequestHeaders.Clear();
});
services.AddHttpClient<AuditLogService, AuditLogService>(client =>
{
    client.BaseAddress = new Uri(appSettings.ResourceServer.Endpoint);
    client.Timeout = new TimeSpan(0, 0, 30);
    client.DefaultRequestHeaders.Clear();
});

services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.Authority = appSettings.OpenIdConnect.Authority;
    options.ClientId = appSettings.OpenIdConnect.ClientId;
    options.ClientSecret = appSettings.OpenIdConnect.ClientSecret;
    options.ResponseType = "code";
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("ClassifiedAds.WebAPI");
    options.Scope.Add("offline_access");
    options.SaveTokens = true;
    options.GetClaimsFromUserInfoEndpoint = true;
    options.TokenValidationParameters.NameClaimType = "name";
    options.RequireHttpsMetadata = appSettings.OpenIdConnect.RequireHttpsMetadata;
});

services.AddHealthChecks()
    .AddHttp(appSettings.OpenIdConnect.Authority, name: "Identity Server", failureStatus: HealthStatus.Degraded)
    .AddHttp(appSettings.ResourceServer.Endpoint, name: "Resource (Web API) Server", failureStatus: HealthStatus.Degraded);

services.Configure<HealthChecksBackgroundServiceOptions>(x => x.Interval = TimeSpan.FromMinutes(10));
services.AddHostedService<HealthChecksBackgroundService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

if (appSettings.CookiePolicyOptions?.IsEnabled ?? false)
{
    app.UseCookiePolicy();
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.UseHealthChecks("/healthz", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = HealthChecksResponseWriter.WriteReponse,
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status500InternalServerError,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable,
    },
});

app.Run();