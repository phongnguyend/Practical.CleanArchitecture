using ClassifiedAds.Application.FileEntries.DTOs;
using ClassifiedAds.CrossCuttingConcerns.Exceptions;
using ClassifiedAds.Domain.Identity;
using ClassifiedAds.Infrastructure.Configuration;
using ClassifiedAds.Infrastructure.HealthChecks;
using ClassifiedAds.Infrastructure.Identity;
using ClassifiedAds.Infrastructure.Logging;
using ClassifiedAds.Infrastructure.Monitoring;
using ClassifiedAds.WebMVC.Authorization;
using ClassifiedAds.WebMVC.ClaimsTransformations;
using ClassifiedAds.WebMVC.ConfigurationOptions;
using ClassifiedAds.WebMVC.Configurations;
using ClassifiedAds.WebMVC.Filters;
using ClassifiedAds.WebMVC.HttpMessageHandlers;
using ClassifiedAds.WebMVC.Middleware;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using System;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

var appSettings = new AppSettings();
configuration.Bind(appSettings);

var validationResult = appSettings.Validate();
if (validationResult.Failed)
{
    throw new ValidationException(validationResult.FailureMessage);
}

builder.WebHost.UseClassifiedAdsLogger(configuration =>
{
    return appSettings.Logging;
});

if (appSettings.CheckDependency.Enabled)
{
    NetworkPortCheck.Wait(appSettings.CheckDependency.Host, 5);
}

builder.Configuration.AddAppConfiguration(appSettings.ConfigurationProviders);

services.TryAddEnumerable(ServiceDescriptor.Singleton<IValidateOptions<AppSettings>, AppSettingsValidation>());

services.Configure<AppSettings>(configuration);

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

services.AddClassifiedAdsSignalR(appSettings);

services.AddMonitoringServices(appSettings.Monitoring);

services.AddControllersWithViews(setupAction =>
{
    setupAction.Filters.Add(typeof(CustomActionFilter));
    setupAction.Filters.Add(typeof(CustomResultFilter));
    setupAction.Filters.Add(typeof(CustomAuthorizationFilter));
    setupAction.Filters.Add(typeof(CustomExceptionFilter));
})
.AddNewtonsoftJson();

services.AddDateTimeProvider();

services.AddPersistence(appSettings.ConnectionStrings.ClassifiedAds)
        .AddDomainServices()
        .AddApplicationServices((Type serviceType, Type implementationType, ServiceLifetime serviceLifetime) =>
        {
            services.AddInterceptors(serviceType, implementationType, serviceLifetime, appSettings.Interceptors);
        })
        .AddMessageHandlers()
        .ConfigureInterceptors();

services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.AccessDeniedPath = "/Authorization/AccessDenied";
})
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
    options.RequireHttpsMetadata = appSettings.OpenIdConnect.RequireHttpsMetadata;
});
services.AddSingleton<IClaimsTransformation, CustomClaimsTransformation>();

services.AddAuthorization(options =>
{
    options.AddPolicy("CustomPolicy", policy =>
    {
        policy.AddRequirements(new CustomRequirement());
    });
})
.AddSingleton<IAuthorizationHandler, CustomRequirementHandler>();

services.AddTransient<ProfilingHttpHandler>();
services.AddTransient<BearerTokenHandler>();
services.AddHttpClient(string.Empty)
        .AddHttpMessageHandler<ProfilingHttpHandler>()
        .AddHttpMessageHandler<BearerTokenHandler>();

services.AddCaches(appSettings.Caching);

var healthChecksBuilder = services.AddHealthChecks()
    .AddSqlServer(connectionString: appSettings.ConnectionStrings.ClassifiedAds,
        healthQuery: "SELECT 1;",
        name: "Sql Server",
        failureStatus: HealthStatus.Degraded)
    .AddHttp(appSettings.OpenIdConnect.Authority,
        name: "Identity Server",
        failureStatus: HealthStatus.Degraded)
    .AddHttp(appSettings.ResourceServer.Endpoint,
        name: "Resource (Web API) Server",
        failureStatus: HealthStatus.Degraded)
    .AddStorageManagerHealthCheck(appSettings.Storage)
    .AddMessageBusHealthCheck(appSettings.MessageBroker);

services.AddHealthChecksUI(setupSettings: setup =>
{
    setup.AddHealthCheckEndpoint("Basic Health Check", $"{appSettings.CurrentUrl}/healthz");
    setup.DisableDatabaseMigrations();
}).AddInMemoryStorage();

services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
services.AddScoped<ICurrentUser, CurrentWebUser>();

services.AddStorageManager(appSettings.Storage);
services.AddMessageBusSender<FileUploadedEvent>(appSettings.MessageBroker);
services.AddMessageBusSender<FileDeletedEvent>(appSettings.MessageBroker);

services.AddFeatureManagement();

var app = builder.Build();

app.UseDebuggingMiddleware();
app.UseMiddleware<CustomMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSecurityHeaders(appSettings.SecurityHeaders);

app.UseIPFiltering();

app.UseHttpsRedirection();

app.UseStaticFiles();

if (appSettings.CookiePolicyOptions?.IsEnabled ?? false)
{
    app.UseCookiePolicy();
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMonitoringServices(appSettings.Monitoring);

app.UseHealthChecks("/healthz", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status500InternalServerError,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable,
    },
});

app.UseHealthChecksUI(); // /healthchecks-ui#/healthchecks

app.MapDefaultControllerRoute();
app.MapClassifiedAdsHubs();

app.Run();
