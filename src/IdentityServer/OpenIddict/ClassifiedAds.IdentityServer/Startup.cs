using ClassifiedAds.Domain.Entities;
using ClassifiedAds.IdentityServer.ConfigurationOptions;
using ClassifiedAds.IdentityServer.HostedServices;
using ClassifiedAds.Infrastructure.Monitoring;
using ClassifiedAds.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace ClassifiedAds.IdentityServer;

public class Startup
{
    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        Configuration = configuration;

        AppSettings = new AppSettings();
        Configuration.Bind(AppSettings);
    }

    public IConfiguration Configuration { get; }

    private AppSettings AppSettings { get; set; }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });

        services.Configure<AppSettings>(Configuration);

        if (AppSettings.CookiePolicyOptions?.IsEnabled ?? false)
        {
            services.Configure<Microsoft.AspNetCore.Builder.CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = AppSettings.CookiePolicyOptions.MinimumSameSitePolicy;
                options.Secure = AppSettings.CookiePolicyOptions.Secure;
            });
        }

        services.AddMonitoringServices(AppSettings.Monitoring);

        services.AddControllersWithViews();
        services.AddRazorPages();

        services.AddCors();

        services.AddDateTimeProvider();

        services.AddDbContext<OpenIddictDbContext>(options =>
        {
            options.UseSqlServer(AppSettings.ConnectionStrings.IdentityServer);

            // Register the entity sets needed by OpenIddict.
            options.UseOpenIddict();
        });

        services.AddPersistence(AppSettings.ConnectionStrings.ClassifiedAds)
                .AddDomainServices()
                .AddApplicationServices((Type serviceType, Type implementationType, ServiceLifetime serviceLifetime) =>
                {
                    services.AddInterceptors(serviceType, implementationType, serviceLifetime, AppSettings.Interceptors);
                })
                .AddMessageHandlers()
                .ConfigureInterceptors()
                .AddIdentity()
                .AddScoped<SignInManager<User>>();

        services.AddHttpContextAccessor();

        services.AddOpenIddict()
        .AddCore(options =>
        {
            options.UseEntityFrameworkCore()
                   .UseDbContext<OpenIddictDbContext>();
        })
        .AddServer(options =>
        {
            // Enable the authorization, logout, token and userinfo endpoints.
            options
                .SetTokenEndpointUris("connect/token")
                .SetAuthorizationEndpointUris("connect/authorize")
                .SetLogoutEndpointUris("connect/logout")
                .SetUserinfoEndpointUris("connect/userinfo");

            options.AllowAuthorizationCodeFlow()
                   .AllowHybridFlow()
                   .AllowClientCredentialsFlow()
                   .AllowPasswordFlow()
                   .AllowRefreshTokenFlow();

            options.RegisterScopes(Scopes.OpenId, Scopes.Profile, Scopes.OfflineAccess, "ClassifiedAds.WebAPI");

            options.AddEncryptionCertificate(AppSettings.IdentityServer.EncryptionCertificate.FindCertificate())
                   .AddSigningCertificate(AppSettings.IdentityServer.SigningCertificate.FindCertificate());

            options
                .UseAspNetCore()
                .EnableTokenEndpointPassthrough()
                .EnableAuthorizationEndpointPassthrough()
                .EnableLogoutEndpointPassthrough()
                .EnableUserinfoEndpointPassthrough();
        })
        .AddValidation(options =>
        {
            options.UseLocalServer();
            options.UseAspNetCore();
        });

        services.AddDataProtection()
            .PersistKeysToDbContext<AdsDbContext>()
            .SetApplicationName("ClassifiedAds");

        services.AddCaches(AppSettings.Caching);

        services.AddHostedService<SeedDataHostedService>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseForwardedHeaders();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseStaticFiles();

        if (AppSettings.CookiePolicyOptions?.IsEnabled ?? false)
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

        app.UseSecurityHeaders(AppSettings.SecurityHeaders);

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMonitoringServices(AppSettings.Monitoring);

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
            endpoints.MapRazorPages();
        });
    }
}
