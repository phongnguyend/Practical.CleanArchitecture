using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.IO;
using Serilog;
using System;
using Microsoft.Extensions.Hosting;

namespace ClassifiedAds.IdentityServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;

            Directory.CreateDirectory(Path.Combine(env.ContentRootPath, "logs"));
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(Path.Combine(env.ContentRootPath, "logs", "log.txt"),
                    fileSizeLimitBytes: 10 * 1024 * 1024,
                    rollOnFileSizeLimit: true,
                    shared: true,
                    flushToDiskInterval: TimeSpan.FromSeconds(1))
                .CreateLogger();
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
                    .AddIdentity();

            services.AddIdentityServer()
                    .AddDeveloperSigningCredential()
                    .AddPersistence(Configuration.GetConnectionString("ClassifiedAds"));

            services.AddMiniProfiler(options =>
            {
                options.RouteBasePath = "/profiler";// access /profiler/results to see last profile check
                options.PopupRenderPosition = StackExchange.Profiling.RenderPosition.BottomLeft;
                options.PopupShowTimeWithChildren = true;
            })
            .AddEntityFramework();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.MigrateIdServerDb();
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

            app.UseMiniProfiler();
            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
