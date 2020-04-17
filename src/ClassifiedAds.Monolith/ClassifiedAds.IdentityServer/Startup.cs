using System.Security.Cryptography.X509Certificates;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Infrastructure.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ClassifiedAds.IdentityServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;

            env.UseClassifiedAdsLogger();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddCors();

            services.AddPersistence(Configuration.GetConnectionString("ClassifiedAds"))
                    .AddDomainServices()
                    .AddApplicationServices()
                    .AddMessageHandlers()
                    .AddIdentity();

            services.AddIdentityServer()
                    .AddSigningCredential(new X509Certificate2(Configuration["Certificates:Default:Path"], Configuration["Certificates:Default:Password"]))
                    .AddAspNetIdentity<User>()
                    .AddIdServerPersistence(Configuration.GetConnectionString("ClassifiedAds"));

            services.AddClassifiedAdsProfiler();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors(
                builder => builder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
            );

            app.UseClassifiedAdsProfiler();
            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
