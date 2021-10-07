using ClassifiedAds.BackgroundServer.ConfigurationOptions;
using ClassifiedAds.BackgroundServer.HostedServices;
using ClassifiedAds.BackgroundServer.Identity;
using ClassifiedAds.Infrastructure.Logging;
using ClassifiedAds.Modules.Identity.Contracts.Services;
using ClassifiedAds.Modules.Notification.ConfigurationOptions;
using ClassifiedAds.Modules.Notification.Contracts.DTOs;
using ClassifiedAds.Modules.Storage.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace ClassifiedAds.BackgroundServer
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

                var validationResult = appSettings.Validate();
                if (validationResult.Failed)
                {
                    throw new Exception(validationResult.FailureMessage);
                }

                services.Configure<AppSettings>(configuration);

                services.AddScoped<ICurrentUser, CurrentUser>();

                services.AddDateTimeProvider();

                services.AddNotificationModule(new NotificationModuleOptions
                {
                    ConnectionStrings = new ConnectionStringsOptions
                    {
                        Default = appSettings.ConnectionStrings.ClassifiedAds,
                    },
                    MessageBroker = appSettings.MessageBroker,
                    Notification = appSettings.Notification,
                })
                .AddApplicationServices();

                services.AddMessageBusReceiver<FileUploadedEvent>(appSettings.MessageBroker);
                services.AddMessageBusReceiver<FileDeletedEvent>(appSettings.MessageBroker);
                services.AddMessageBusReceiver<EmailMessageCreatedEvent>(appSettings.MessageBroker);
                services.AddMessageBusReceiver<SmsMessageCreatedEvent>(appSettings.MessageBroker);

                services.AddHostedService<SendEmailHostedService>();
                services.AddHostedService<SendSmsHostedService>();
            });
    }
}
