using ClassifiedAds.Infrastructure.HealthChecks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ClassifiedAds.Migrator
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
            if (string.Equals(Configuration["CheckDependency:Enabled"], "true", System.StringComparison.OrdinalIgnoreCase))
            {
                NetworkPortCheck.Wait(Configuration["CheckDependency:Host"], 5);
            }

            services.AddPersistence(Configuration["ConnectionStrings:ClassifiedAds"],
                typeof(Startup).GetTypeInfo().Assembly.GetName().Name);

            services.AddIdentityServer()
                .AddIdServerPersistence(Configuration.GetConnectionString("ClassifiedAds"),
                typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.MigrateDb();
            app.MigrateIdServerDb();
        }
    }
}
