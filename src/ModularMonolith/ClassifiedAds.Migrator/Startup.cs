using ClassifiedAds.Infrastructure.HealthChecks;
using ClassifiedAds.Infrastructure.MessageBrokers;
using ClassifiedAds.Infrastructure.Notification;
using ClassifiedAds.Infrastructure.Notification.Email;
using ClassifiedAds.Infrastructure.Notification.Sms;
using ClassifiedAds.Infrastructure.Notification.Web;
using ClassifiedAds.Infrastructure.Storages;
using ClassifiedAds.Modules.Configuration.ConfigurationOptions;
using ClassifiedAds.Modules.Identity.ConfigurationOptions;
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

            var messageBrokerOptions = new MessageBrokerOptions
            {
                Provider = "Fake",
            };

            var notificationOptions = new NotificationOptions
            {
                Email = new EmailOptions { Provider = "Fake" },
                Sms = new SmsOptions { Provider = "Fake" },
                Web = new WebOptions { Provider = "Fake" },
            };

            services.AddAuditLogModule(new Modules.AuditLog.ConfigurationOptions.AuditLogModuleOptions
            {
                ConnectionStrings = new Modules.AuditLog.ConfigurationOptions.ConnectionStringsOptions
                {
                    Default = Configuration["ConnectionStrings:ClassifiedAds"],
                    MigrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name,
                },
            })
            .AddConfigurationModule(new ConfigurationModuleOptions
            {
                ConnectionStrings = new Modules.Configuration.ConfigurationOptions.ConnectionStringsOptions
                {
                    Default = Configuration["ConnectionStrings:ClassifiedAds"],
                    MigrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name,
                },
            })
            .AddIdentityModule(new IdentityModuleOptions
            {
                ConnectionStrings = new Modules.Identity.ConfigurationOptions.ConnectionStringsOptions
                {
                    Default = Configuration["ConnectionStrings:ClassifiedAds"],
                    MigrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name,
                },
            })
            .AddNotificationModule(new Modules.Notification.ConfigurationOptions.NotificationModuleOptions
            {
                ConnectionStrings = new Modules.Notification.ConfigurationOptions.ConnectionStringsOptions
                {
                    Default = Configuration["ConnectionStrings:ClassifiedAds"],
                    MigrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name,
                },
                MessageBroker = messageBrokerOptions,
                Notification = notificationOptions,
            })
            .AddProductModule(new Modules.Product.ConfigurationOptions.ProductModuleOptions
            {
                ConnectionStrings = new Modules.Product.ConfigurationOptions.ConnectionStringsOptions
                {
                    Default = Configuration["ConnectionStrings:ClassifiedAds"],
                    MigrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name,
                },
            })
            .AddStorageModule(new Modules.Storage.ConfigurationOptions.StorageModuleOptions
            {
                ConnectionStrings = new Modules.Storage.ConfigurationOptions.ConnectionStringsOptions
                {
                    Default = Configuration["ConnectionStrings:ClassifiedAds"],
                    MigrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name,
                },
                Storage = new StorageOptions(),
                MessageBroker = messageBrokerOptions,
            })
            .AddApplicationServices();

            services.AddIdentityServer()
                .AddTokenProviderModule(Configuration.GetConnectionString("ClassifiedAds"),
                typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
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
                .SqlDatabase(Configuration.GetConnectionString("ClassifiedAds"))
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
