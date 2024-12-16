using ClassifiedAds.Application.ConfigurationEntries.DTOs;
using ClassifiedAds.Application.Products.DTOs;
using ClassifiedAds.CrossCuttingConcerns.Csv;
using ClassifiedAds.CrossCuttingConcerns.Excel;
using ClassifiedAds.Domain.Identity;
using ClassifiedAds.Infrastructure.Csv;
using ClassifiedAds.Infrastructure.Excel.ClosedXML;
using ClassifiedAds.Infrastructure.HealthChecks;
using ClassifiedAds.Infrastructure.HostedServices;
using ClassifiedAds.Infrastructure.Identity;
using ClassifiedAds.Infrastructure.Localization;
using ClassifiedAds.Infrastructure.Logging;
using ClassifiedAds.Infrastructure.Monitoring;
using ClassifiedAds.Infrastructure.Web.Endpoints;
using ClassifiedAds.Infrastructure.Web.ExceptionHandlers;
using ClassifiedAds.Persistence;
using ClassifiedAds.WebAPI.Authorization;
using ClassifiedAds.WebAPI.ConfigurationOptions;
using ClassifiedAds.WebAPI.Hubs;
using ClassifiedAds.WebAPI.RateLimiterPolicies;
using ClassifiedAds.WebAPI.Tenants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;
var configuration = builder.Configuration;

var appSettings = new AppSettings();
configuration.Bind(appSettings);

builder.WebHost.UseClassifiedAdsLogger(configuration =>
{
    return appSettings.Logging;
});

services.Configure<AppSettings>(configuration);

services.AddMonitoringServices(appSettings.Monitoring);

services.AddExceptionHandler<GlobalExceptionHandler>();

services.AddControllers(configure =>
{
})
.ConfigureApiBehaviorOptions(options =>
{
})
.AddJsonOptions(options =>
{
});

services.AddSignalR();

services.AddCors(options =>
{
    options.AddPolicy("AllowedOrigins", builder => builder
        .WithOrigins(appSettings.CORS.AllowedOrigins)
        .AllowAnyMethod()
        .AllowAnyHeader());

    options.AddPolicy("SignalRHubs", builder => builder
        .WithOrigins(appSettings.CORS.AllowedOrigins)
        .AllowAnyHeader()
        .WithMethods("GET", "POST")
        .AllowCredentials());

    options.AddPolicy("AllowAnyOrigin", builder => builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());

    options.AddPolicy("CustomPolicy", builder => builder
        .AllowAnyOrigin()
        .WithMethods("Get")
        .WithHeaders("Content-Type"));
});

services.AddDateTimeProvider();

services.AddMultiTenantPersistence(typeof(AdsDbContextMultiTenantConnectionStringResolver), typeof(TenantResolver))
        .AddDomainServices()
        .AddApplicationServices((Type serviceType, Type implementationType, ServiceLifetime serviceLifetime) =>
        {
            services.AddInterceptors(serviceType, implementationType, serviceLifetime, appSettings.Interceptors);
        })
        .AddMessageHandlers()
        .ConfigureInterceptors()
        .AddIdentityCore();

services.AddDataProtection()
    .PersistKeysToDbContext<AdsDbContext>()
    .SetApplicationName("ClassifiedAds");

services.AddAuthentication(options =>
{
    options.DefaultScheme = appSettings.IdentityServerAuthentication.Provider switch
    {
        "OpenIddict" => "OpenIddict",
        _ => JwtBearerDefaults.AuthenticationScheme
    };
})
.AddJwtBearer(options =>
{
    options.Authority = appSettings.IdentityServerAuthentication.Authority;
    options.Audience = appSettings.IdentityServerAuthentication.ApiName;
    options.RequireHttpsMetadata = appSettings.IdentityServerAuthentication.RequireHttpsMetadata;
})
.AddJwtBearer("OpenIddict", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false,
        ValidIssuer = appSettings.IdentityServerAuthentication.OpenIddict.IssuerUri,
        TokenDecryptionKey = new X509SecurityKey(appSettings.IdentityServerAuthentication.OpenIddict.TokenDecryptionCertificate.FindCertificate()),
        IssuerSigningKey = new X509SecurityKey(appSettings.IdentityServerAuthentication.OpenIddict.IssuerSigningCertificate.FindCertificate()),
    };
});

services.AddAuthorizationPolicies(Assembly.GetExecutingAssembly(), AuthorizationPolicyNames.GetPolicyNames());

services.AddRateLimiter(options =>
{
    options.AddPolicy<string, DefaultRateLimiterPolicy>(RateLimiterPolicyNames.DefaultPolicy);
    options.AddPolicy<string, GetAuditLogsRateLimiterPolicy>(RateLimiterPolicyNames.GetAuditLogsPolicy);
});

services.AddSwaggerGen(setupAction =>
{
    setupAction.SwaggerDoc(
        $"ClassifiedAds",
        new OpenApiInfo()
        {
            Title = "ClassifiedAds API",
            Version = "1",
            Description = "ClassifiedAds API Specification.",
            Contact = new OpenApiContact
            {
                Email = "abc.xyz@gmail.com",
                Name = "Phong Nguyen",
                Url = new Uri("https://github.com/phongnguyend"),
            },
            License = new OpenApiLicense
            {
                Name = "MIT License",
                Url = new Uri("https://opensource.org/licenses/MIT"),
            },
        });

    setupAction.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "Input your Bearer token to access this API",
    });

    setupAction.AddSecurityDefinition("Oidc", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                TokenUrl = new Uri(appSettings.IdentityServerAuthentication.Authority + "/connect/token", UriKind.Absolute),
                AuthorizationUrl = new Uri(appSettings.IdentityServerAuthentication.Authority + "/connect/authorize", UriKind.Absolute),
                Scopes = new Dictionary<string, string>
                {
                            { "openid", "OpenId" },
                            { "profile", "Profile" },
                            { "ClassifiedAds.WebAPI", "ClassifiedAds WebAPI" },
                },
            },
            ClientCredentials = new OpenApiOAuthFlow
            {
                TokenUrl = new Uri(appSettings.IdentityServerAuthentication.Authority + "/connect/token", UriKind.Absolute),
                Scopes = new Dictionary<string, string>
                {
                            { "ClassifiedAds.WebAPI", "ClassifiedAds WebAPI" },
                },
            },
        },
    });

    setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Oidc",
                },
            }, new List<string>()
        },
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer",
                },
            }, new List<string>()
        },
    });
});

services.AddCaches(appSettings.Caching);

services.AddHealthChecks()
    .AddSqlServer(connectionString: appSettings.ConnectionStrings.ClassifiedAds,
        healthQuery: "SELECT 1;",
        name: "Sql Server",
        failureStatus: HealthStatus.Degraded)
    .AddHttp(appSettings.IdentityServerAuthentication.Authority,
        name: "Identity Server",
        failureStatus: HealthStatus.Degraded)
    .AddStorageManagerHealthCheck(appSettings.Storage);

services.Configure<HealthChecksBackgroundServiceOptions>(x => x.Interval = TimeSpan.FromMinutes(10));
services.AddHostedService<HealthChecksBackgroundService>();

services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
services.AddScoped<ICurrentUser, CurrentWebUser>();

services.AddStorageManager(appSettings.Storage);
services.AddHtmlWriters();
services.AddDinkToPdfWriters();
services.AddClassifiedAdsLocalization();

services.AddScoped<ICsvReader<ImportProductsFromCsv>, ImportProductsFromCsvHandler>();
services.AddScoped<ICsvWriter<ExportProductsToCsv>, ExportProductsToCsvHandler>();
services.AddScoped<IExcelReader<ImportConfigurationEntriesFromExcel>, ImportConfigurationEntriesFromExcelHandler>();
services.AddScoped<IExcelWriter<ExportConfigurationEntriesToExcel>, ExportConfigurationEntriesToExcelHandler>();

// Configure the HTTP request pipeline.
var app = builder.Build();

app.UseDebuggingMiddleware();

app.UseAccessTokenFromFormMiddleware();

app.UseLoggingStatusCodeMiddleware();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseExceptionHandler(options => { });

app.UseRouting();

app.UseCors(appSettings.CORS.AllowAnyOrigin ? "AllowAnyOrigin" : "AllowedOrigins");

app.UseSecurityHeaders(appSettings.SecurityHeaders);

app.UseSwagger();

app.UseSwaggerUI(setupAction =>
{
    setupAction.OAuthClientId("Swagger");
    setupAction.OAuthClientSecret("secret");
    setupAction.OAuthUsePkce();

    setupAction.SwaggerEndpoint(
        "/swagger/ClassifiedAds/swagger.json",
        "ClassifiedAds API");

    setupAction.RoutePrefix = string.Empty;

    setupAction.DefaultModelExpandDepth(2);
    setupAction.DefaultModelRendering(ModelRendering.Model);
    setupAction.DocExpansion(DocExpansion.None);
    setupAction.EnableDeepLinking();
    setupAction.DisplayOperationId();
});

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

app.UseMonitoringServices(appSettings.Monitoring);

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

app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notification").RequireCors("SignalRHubs");

app.MapProcessInforEndpoint();
app.MapThreadPoolInforEndpoint();
app.MapGcInforEndpoint();

app.Run();