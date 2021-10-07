using ClassifiedAds.BackgroundServer.Identity;
using ClassifiedAds.Infrastructure.Identity;
using ClassifiedAds.Infrastructure.Logging;
using ClassifiedAds.Services.Notification.Background.HostedServices;
using ClassifiedAds.Services.Notification.ConfigurationOptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ClassifiedAds.Services.Notification.Background
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseWindowsService()
            .UseClassifiedAdsLogger(configuration =>
            {
                var appSettings = new AppSettings();
                configuration.Bind(appSettings);
                return appSettings.Logging;
            })
            .ConfigureServices((hostContext, services) =>
            {
                var serviceProvider = services.BuildServiceProvider();
                var configuration = serviceProvider.GetService<IConfiguration>();

                var appSettings = new AppSettings();
                configuration.Bind(appSettings);

                services.AddDateTimeProvider();
                services.AddApplicationServices();
                services.AddNotificationModule(appSettings);

                services.AddScoped<ICurrentUser, CurrentUser>();
                services.AddHostedService<SendEmailHostedService>();
                services.AddHostedService<SendSmsHostedService>();
            });
    }
}
