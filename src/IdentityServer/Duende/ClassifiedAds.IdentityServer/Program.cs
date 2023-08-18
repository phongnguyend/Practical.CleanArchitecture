using ClassifiedAds.Domain.Entities;
using ClassifiedAds.IdentityServer.ConfigurationOptions;
using ClassifiedAds.Infrastructure.Logging;
using ClassifiedAds.Infrastructure.Monitoring;
using ClassifiedAds.Persistence;
using Duende.IdentityServer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;

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

services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

services.Configure<AppSettings>(configuration);

var appSettings = new AppSettings();
configuration.Bind(appSettings);

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

services.AddMonitoringServices(appSettings.Monitoring);

services.AddRazorPages();

services.AddCors();

services.AddDateTimeProvider();

services.AddPersistence(appSettings.ConnectionStrings.ClassifiedAds)
        .AddDomainServices()
        .AddApplicationServices((Type serviceType, Type implementationType, ServiceLifetime serviceLifetime) =>
        {
            services.AddInterceptors(serviceType, implementationType, serviceLifetime, appSettings.Interceptors);
        })
        .AddMessageHandlers()
        .ConfigureInterceptors()
        .AddIdentity();

services.AddIdentityServer(options =>
{
    if (!string.IsNullOrWhiteSpace(appSettings.IdentityServer.IssuerUri))
    {
        options.IssuerUri = appSettings.IdentityServer.IssuerUri;
    }

    options.InputLengthRestrictions.Password = int.MaxValue;
    options.InputLengthRestrictions.UserName = int.MaxValue;
})
        .AddSigningCredential(appSettings.IdentityServer.Certificate.FindCertificate())
        .AddAspNetIdentity<User>()
        .AddIdServerPersistence(appSettings.ConnectionStrings.IdentityServer);

services.AddDataProtection()
    .PersistKeysToDbContext<AdsDbContext>()
    .SetApplicationName("ClassifiedAds");

services.AddCaches(appSettings.Caching);

var authenBuilder = services.AddAuthentication();

if (appSettings.ExternalLogin?.AzureActiveDirectory?.IsEnabled ?? false)
{
    authenBuilder.AddOpenIdConnect("AAD", "Azure Active Directory", options =>
    {
        options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
        options.SignOutScheme = IdentityServerConstants.SignoutScheme;
        options.Authority = appSettings.ExternalLogin.AzureActiveDirectory.Authority;
        options.ClientId = appSettings.ExternalLogin.AzureActiveDirectory.ClientId;
        options.ClientSecret = appSettings.ExternalLogin.AzureActiveDirectory.ClientSecret;
        options.ResponseType = "code id_token";
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");
        options.Scope.Add("offline_access");
        options.SaveTokens = true;
        options.GetClaimsFromUserInfoEndpoint = true;
        options.Events.OnTicketReceived = (ct) =>
        {
            return Task.CompletedTask;
        };
        options.Events.OnTokenResponseReceived = (ct) =>
        {
            return Task.CompletedTask;
        };
        options.Events.OnTokenValidated = (ct) =>
        {
            return Task.CompletedTask;
        };
        options.Events.OnUserInformationReceived = (ct) =>
        {
            return Task.CompletedTask;
        };
    });
}

if (appSettings?.ExternalLogin?.Microsoft?.IsEnabled ?? false)
{
    authenBuilder.AddMicrosoftAccount(options =>
    {
        options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
        options.ClientId = appSettings.ExternalLogin.Microsoft.ClientId;
        options.ClientSecret = appSettings.ExternalLogin.Microsoft.ClientSecret;
    });
}

if (appSettings?.ExternalLogin?.Google?.IsEnabled ?? false)
{
    authenBuilder.AddGoogle(options =>
    {
        options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
        options.ClientId = appSettings.ExternalLogin.Google.ClientId;
        options.ClientSecret = appSettings.ExternalLogin.Google.ClientSecret;
    });
}

if (appSettings?.ExternalLogin?.Facebook?.IsEnabled ?? false)
{
    authenBuilder.AddFacebook(options =>
    {
        options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
        options.AppId = appSettings.ExternalLogin.Facebook.AppId;
        options.AppSecret = appSettings.ExternalLogin.Facebook.AppSecret;
    });
}

// Configure the HTTP request pipeline.
var app = builder.Build();

app.UseForwardedHeaders();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();

if (appSettings.CookiePolicyOptions?.IsEnabled ?? false)
{
    app.UseCookiePolicy();
}

app.UseRouting();

app.UseCors(
    builder => builder
    .AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod()
);

app.UseSecurityHeaders(appSettings.SecurityHeaders);

app.UseIdentityServer();
app.UseAuthorization();

app.UseMonitoringServices(appSettings.Monitoring);

app.MapRazorPages();

app.Run();
