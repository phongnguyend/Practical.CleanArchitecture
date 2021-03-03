﻿using ClassifiedAds.IdentityServer.ConfigurationOptions;
using ClassifiedAds.Infrastructure.Monitoring;
using ClassifiedAds.Services.Identity.Entities;
using ClassifiedAds.Services.Identity.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

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

            services.AddControllersWithViews();

            services.AddCors();

            services.AddDateTimeProvider();

            services.AddIdentityModule(AppSettings)
                    .AddApplicationServices();

            services.AddIdentityServer()
                    .AddSigningCredential(AppSettings.Certificates.Default.FindCertificate())
                    .AddAspNetIdentity<User>()
                    .AddTokenProviderModule(AppSettings.ConnectionStrings.ClassifiedAds, typeof(Startup).GetTypeInfo().Assembly.GetName().Name);

            services.AddDataProtection()
                .PersistKeysToDbContext<IdentityDbContext>()
                .SetApplicationName("ClassifiedAds");

            services.AddCaches(AppSettings.Caching)
                    .AddMonitoringServices(AppSettings.Monitoring);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.MigrateIdServerDb();

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
            });
        }
    }
}
