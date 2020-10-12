using ClassifiedAds.Infrastructure.HealthChecks;
using ClassifiedAds.Infrastructure.MessageBrokers;
using ClassifiedAds.Infrastructure.Notification;
using ClassifiedAds.Infrastructure.Notification.Email;
using ClassifiedAds.Infrastructure.Notification.Sms;
using ClassifiedAds.Infrastructure.Notification.Web;
using ClassifiedAds.Infrastructure.Storages;
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

            services.AddDateTimeProvider();

            var messageBrokerOptions = new MessageBrokerOptions { Provider = "Fake" };
            var notificationOptions = new NotificationOptions
            {
                Email = new EmailOptions { Provider = "Fake" },
                Sms = new SmsOptions { Provider = "Fake" },
                Web = new WebOptions { Provider = "Fake" },
            };

            services.AddAuditLogModule(Configuration["ConnectionStrings:ClassifiedAds"],
                typeof(Startup).GetTypeInfo().Assembly.GetName().Name)
                .AddIdentityModule(Configuration["ConnectionStrings:ClassifiedAds"],
                typeof(Startup).GetTypeInfo().Assembly.GetName().Name)
                .AddNotificationModule(messageBrokerOptions, notificationOptions, Configuration["ConnectionStrings:ClassifiedAds"],
                typeof(Startup).GetTypeInfo().Assembly.GetName().Name)
                .AddProductModule(Configuration["ConnectionStrings:ClassifiedAds"],
                typeof(Startup).GetTypeInfo().Assembly.GetName().Name)
                .AddStorageModule(new StorageOptions(),
                messageBrokerOptions,
                Configuration["ConnectionStrings:ClassifiedAds"],
                typeof(Startup).GetTypeInfo().Assembly.GetName().Name)
                .AddApplicationServices();

            services.AddIdentityServer()
                .AddTokenProviderModule(Configuration.GetConnectionString("ClassifiedAds"),
                typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.MigrateAuditLogDb();
            app.MigrateIdentityDb();
            app.MigrateNotificationDb();
            app.MigrateProductDb();
            app.MigrateStorageDb();
            app.MigrateIdServerDb();
        }
    }
}
