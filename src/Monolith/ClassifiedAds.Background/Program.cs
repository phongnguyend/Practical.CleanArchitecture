using ClassifiedAds.Application.FileEntries.DTOs;
using ClassifiedAds.Background.ConfigurationOptions;
using ClassifiedAds.Background.HostedServices;
using ClassifiedAds.Background.Identity;
using ClassifiedAds.Background.MessageBusConsumers;
using ClassifiedAds.CrossCuttingConcerns.Exceptions;
using ClassifiedAds.Domain.Identity;
using ClassifiedAds.Domain.IdentityProviders;
using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using ClassifiedAds.Infrastructure.HostedServices;
using ClassifiedAds.Infrastructure.IdentityProviders.Auth0;
using ClassifiedAds.Infrastructure.IdentityProviders.Azure;
using ClassifiedAds.Infrastructure.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;

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
        throw new ValidationException(validationResult.FailureMessage);
    }

    services.Configure<AppSettings>(configuration);

    services.AddScoped<ICurrentUser, CurrentUser>();

    services.AddDateTimeProvider();
    services.AddPersistence(appSettings.ConnectionStrings.ClassifiedAds)
            .AddDomainServices()
            .AddApplicationServices()
            .AddMessageHandlers();

    services.AddTransient<IMessageBus, MessageBus>();
    services.AddMessageBusSender<FileUploadedEvent>(appSettings.MessageBroker);
    services.AddMessageBusSender<FileDeletedEvent>(appSettings.MessageBroker);
    services.AddMessageBusReceiver<WebhookConsumer, FileUploadedEvent>(appSettings.MessageBroker);
    services.AddMessageBusReceiver<WebhookConsumer, FileDeletedEvent>(appSettings.MessageBroker);
    services.AddMessageBusConsumers(Assembly.GetExecutingAssembly());
    services.AddOutboxEventPublishers(Assembly.GetExecutingAssembly());

    services.AddNotificationServices(appSettings.Notification);

    services.AddWebNotification<SendTaskStatusMessage>(appSettings.Notification.Web);

    if (appSettings.IdentityProviders?.Auth0?.Enabled ?? false)
    {
        services.AddSingleton<IAuth0IdentityProvider>(new Auth0IdentityProvider(appSettings.IdentityProviders.Auth0));
    }

    if (appSettings.IdentityProviders?.AzureActiveDirectoryB2C?.Enabled ?? false)
    {
        services.AddSingleton<IAzureActiveDirectoryB2CIdentityProvider>(new AzureActiveDirectoryB2CIdentityProvider(appSettings.IdentityProviders.AzureActiveDirectoryB2C));
    }

    services.AddHealthChecks()
    .AddSqlServer(connectionString: appSettings.ConnectionStrings.ClassifiedAds,
        healthQuery: "SELECT 1;",
        name: "Sql Server",
        failureStatus: HealthStatus.Degraded)
    .AddMessageBusHealthCheck(appSettings.MessageBroker);

    services.Configure<HealthChecksBackgroundServiceOptions>(x => x.Interval = TimeSpan.FromMinutes(10));
    services.AddHostedService<HealthChecksBackgroundService>();

    services.AddHostedService<MessageBusConsumerBackgroundService<WebhookConsumer, FileUploadedEvent>>();
    services.AddHostedService<MessageBusConsumerBackgroundService<WebhookConsumer, FileDeletedEvent>>();
    services.AddHostedService<PublishEventWorker>();
    services.AddHostedService<SendEmailWorker>();
    services.AddHostedService<SendSmsWorker>();
    services.AddHostedService<ScheduleCronJobWorker>();
    services.AddHostedService<SyncUsersWorker>();
})
.Build()
.Run();
