using ClassifiedAds.NotificationServer.ConfigurationOptions;
using ClassifiedAds.NotificationServer.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ClassifiedAds.NotificationServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var appSettings = new AppSettings();
            Configuration.Bind(appSettings);

            var signalR = services.AddSignalR();

            if (appSettings.Azure?.SignalR?.IsEnabled ?? false)
            {
                signalR.AddAzureSignalR();
            }

            signalR.AddMessagePackProtocol();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowedOrigins", builder => builder
                    .WithOrigins(appSettings.CORS.AllowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseCors("AllowedOrigins");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<HealthCheckHub>("/HealthCheckHub");
                endpoints.MapHub<SimulatedLongRunningTaskHub>("/SimulatedLongRunningTaskHub");
            });
        }
    }
}
