using ClassifiedAds.Infrastructure.HealthChecks;
using ClassifiedAds.Modules.Identity.Contracts.Services;
using ClassifiedAds.Modules.Identity.Services;
using DbUp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using System;
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

            services.AddDateTimeProvider();

            services.AddAuditLogModule(opt =>
            {
                Configuration.GetSection("Modules:AuditLog").Bind(opt);
                opt.ConnectionStrings.MigrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            })
            .AddConfigurationModule(opt =>
            {
                Configuration.GetSection("Modules:Configuration").Bind(opt);
                opt.ConnectionStrings.MigrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            })
            .AddIdentityModuleCore(opt =>
            {
                Configuration.GetSection("Modules:Identity").Bind(opt);
                opt.ConnectionStrings.MigrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            })
            .AddNotificationModule(opt =>
            {
                Configuration.GetSection("Modules:Notification").Bind(opt);
                opt.ConnectionStrings.MigrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            })
            .AddProductModule(opt =>
            {
                Configuration.GetSection("Modules:Product").Bind(opt);
                opt.ConnectionStrings.MigrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            })
            .AddStorageModule(opt =>
            {
                Configuration.GetSection("Modules:Storage").Bind(opt);
                opt.ConnectionStrings.MigrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            })
            .AddApplicationServices();

            services.AddIdentityServer()
                .AddIdServerPersistence(Configuration["Modules:Auth:ConnectionStrings:Default"],
                typeof(Startup).GetTypeInfo().Assembly.GetName().Name);

            services.AddScoped<ICurrentUser, CurrentWebUser>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Policy.Handle<Exception>().WaitAndRetry(new[]
            {
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(20),
                TimeSpan.FromSeconds(30),
            })
            .Execute(() =>
            {
                app.MigrateAuditLogDb();
                app.MigrateConfigurationDb();
                app.MigrateIdentityDb();
                app.MigrateNotificationDb();
                app.MigrateProductDb();
                app.MigrateStorageDb();
                app.MigrateIdServerDb();

                var upgrader = DeployChanges.To
                .SqlDatabase(Configuration["Modules:Auth:ConnectionStrings:Default"])
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .LogToConsole()
                .Build();

                var result = upgrader.PerformUpgrade();

                if (!result.Successful)
                {
                    throw result.Error;
                }
            });

        }
    }
}
