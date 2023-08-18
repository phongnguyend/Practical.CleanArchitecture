using ClassifiedAds.Domain.Entities;
using ClassifiedAds.IdentityServer.ConfigurationOptions;
using ClassifiedAds.Infrastructure.Monitoring;
using ClassifiedAds.Persistence;
using IdentityServer4;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace ClassifiedAds.IdentityServer
{
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

            services.AddPersistence(AppSettings.ConnectionStrings.ClassifiedAds)
                    .AddDomainServices()
                    .AddApplicationServices((Type serviceType, Type implementationType, ServiceLifetime serviceLifetime) =>
                    {
                        services.AddInterceptors(serviceType, implementationType, serviceLifetime, AppSettings.Interceptors);
                    })
                    .AddMessageHandlers()
                    .ConfigureInterceptors()
                    .AddIdentity();

            services.AddIdentityServer(options =>
                    {
                        if (!string.IsNullOrWhiteSpace(AppSettings.IdentityServer.IssuerUri))
                        {
                            options.IssuerUri = AppSettings.IdentityServer.IssuerUri;
                        }

                        options.InputLengthRestrictions.Password = int.MaxValue;
                        options.InputLengthRestrictions.UserName = int.MaxValue;
                    })
                    .AddSigningCredential(AppSettings.IdentityServer.Certificate.FindCertificate())
                    .AddAspNetIdentity<User>()
                    .AddIdServerPersistence(AppSettings.ConnectionStrings.IdentityServer);

            services.AddDataProtection()
                .PersistKeysToDbContext<AdsDbContext>()
                .SetApplicationName("ClassifiedAds");

            services.AddCaches(AppSettings.Caching);

            var authenBuilder = services.AddAuthentication();

            if (AppSettings?.ExternalLogin?.AzureActiveDirectory?.IsEnabled ?? false)
            {
                authenBuilder.AddOpenIdConnect("AAD", "Azure Active Directory", options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.SignOutScheme = IdentityServerConstants.SignoutScheme;
                    options.Authority = AppSettings.ExternalLogin.AzureActiveDirectory.Authority;
                    options.ClientId = AppSettings.ExternalLogin.AzureActiveDirectory.ClientId;
                    options.ClientSecret = AppSettings.ExternalLogin.AzureActiveDirectory.ClientSecret;
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

            if (AppSettings?.ExternalLogin?.Microsoft?.IsEnabled ?? false)
            {
                authenBuilder.AddMicrosoftAccount(options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.ClientId = AppSettings.ExternalLogin.Microsoft.ClientId;
                    options.ClientSecret = AppSettings.ExternalLogin.Microsoft.ClientSecret;
                });
            }

            if (AppSettings?.ExternalLogin?.Google?.IsEnabled ?? false)
            {
                authenBuilder.AddGoogle(options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.ClientId = AppSettings.ExternalLogin.Google.ClientId;
                    options.ClientSecret = AppSettings.ExternalLogin.Google.ClientSecret;
                });
            }

            if (AppSettings?.ExternalLogin?.Facebook?.IsEnabled ?? false)
            {
                authenBuilder.AddFacebook(options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.AppId = AppSettings.ExternalLogin.Facebook.AppId;
                    options.AppSecret = AppSettings.ExternalLogin.Facebook.AppSecret;
                });
            }
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

            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseMonitoringServices(AppSettings.Monitoring);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });
        }
    }
}
