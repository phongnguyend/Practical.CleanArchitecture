using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.IO;
using Serilog;
using System;

namespace ClassifiedAds.IdentityServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
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
            services.AddMvc();

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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.MigrateIdServerDb();
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(
                builder => builder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
            );

            app.UseMiniProfiler();
            app.UseIdentityServer();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}
