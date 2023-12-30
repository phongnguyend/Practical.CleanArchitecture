using ClassifiedAds.Contracts.Identity.Services;
using ClassifiedAds.Infrastructure.Logging;
using ClassifiedAds.Infrastructure.Monitoring;
using ClassifiedAds.Infrastructure.Web.ExceptionHandlers;
using ClassifiedAds.Modules.Identity.Repositories;
using ClassifiedAds.Modules.Identity.Services;
using ClassifiedAds.Modules.Notification.Hubs;
using ClassifiedAds.WebAPI.ConfigurationOptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;
var configuration = builder.Configuration;

builder.WebHost.UseClassifiedAdsLogger(configuration =>
{
    var appSettings = new AppSettings();
    configuration.Bind(appSettings);
    return appSettings.Logging;
});

var appSettings = new AppSettings();
configuration.Bind(appSettings);

services.Configure<AppSettings>(configuration);

services.AddExceptionHandler<GlobalExceptionHandler>();

services.AddControllers(configure =>
{
})
.ConfigureApiBehaviorOptions(options =>
{
})
.AddJsonOptions(options =>
{
})
.AddAuditLogModule()
.AddConfigurationModule()
.AddIdentityModule()
.AddNotificationModule()
.AddProductModule()
.AddStorageModule();

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

services.AddAuditLogModule(opt => configuration.GetSection("Modules:AuditLog").Bind(opt))
.AddConfigurationModule(opt => configuration.GetSection("Modules:Configuration").Bind(opt))
.AddIdentityModuleCore(opt => configuration.GetSection("Modules:Identity").Bind(opt))
.AddNotificationModule(opt => configuration.GetSection("Modules:Notification").Bind(opt))
.AddProductModule(opt => configuration.GetSection("Modules:Product").Bind(opt))
.AddStorageModule(opt => configuration.GetSection("Modules:Storage").Bind(opt))
.AddApplicationServices();

services.AddHtmlGenerator();
services.AddDinkToPdfConverter();

services.AddDataProtection()
    .PersistKeysToDbContext<IdentityDbContext>()
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

services.AddMonitoringServices(appSettings.Monitoring);

services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
services.AddScoped<ICurrentUser, CurrentWebUser>();

// Configure the HTTP request pipeline.
var app = builder.Build();

app.UseDebuggingMiddleware();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseExceptionHandler(options => { });

app.UseRouting();

app.UseCors(appSettings.CORS.AllowAnyOrigin ? "AllowAnyOrigin" : "AllowedOrigins");

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

app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notification").RequireCors("SignalRHubs");

app.Run();
