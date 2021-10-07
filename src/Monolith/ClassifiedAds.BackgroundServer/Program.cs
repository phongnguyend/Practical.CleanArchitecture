using ClassifiedAds.Application.EmailMessages.DTOs;
using ClassifiedAds.Application.FileEntries.DTOs;
using ClassifiedAds.Application.SmsMessages.DTOs;
using ClassifiedAds.BackgroundServer.ConfigurationOptions;
using ClassifiedAds.BackgroundServer.HostedServices;
using ClassifiedAds.BackgroundServer.Identity;
using ClassifiedAds.Domain.Identity;
using ClassifiedAds.Infrastructure.Logging;
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
                services.AddPersistence(appSettings.ConnectionStrings.ClassifiedAds)
                        .AddDomainServices()
                        .AddApplicationServices();

                services.AddMessageBusSender<FileUploadedEvent>(appSettings.MessageBroker);
                services.AddMessageBusSender<FileDeletedEvent>(appSettings.MessageBroker);
                services.AddMessageBusSender<EmailMessageCreatedEvent>(appSettings.MessageBroker);
                services.AddMessageBusSender<SmsMessageCreatedEvent>(appSettings.MessageBroker);

                services.AddMessageBusReceiver<FileUploadedEvent>(appSettings.MessageBroker);
                services.AddMessageBusReceiver<FileDeletedEvent>(appSettings.MessageBroker);
                services.AddMessageBusReceiver<EmailMessageCreatedEvent>(appSettings.MessageBroker);
                services.AddMessageBusReceiver<SmsMessageCreatedEvent>(appSettings.MessageBroker);

                services.AddNotificationServices(appSettings.Notification);

                services.AddWebNotification<SendTaskStatusMessage>(appSettings.Notification.Web);

                services.AddHostedService<SendEmailHostedService>();
                services.AddHostedService<SendSmsHostedService>();
                services.AddHostedService<ScheduleCronJobHostedService>();
            });
    }
}
