using ClassifiedAds.Application.FeatureToggles;
using ClassifiedAds.Application.FileEntries.MessageBusEvents;
using ClassifiedAds.Application.Products.MessageBusEvents;
using ClassifiedAds.Background.ConfigurationOptions;
using ClassifiedAds.Background.HostedServices;
using ClassifiedAds.Background.Identity;
using ClassifiedAds.Background.MessageBusConsumers;
using ClassifiedAds.Background.Services;
using ClassifiedAds.CrossCuttingConcerns.Exceptions;
using ClassifiedAds.Domain.Identity;
using ClassifiedAds.Domain.IdentityProviders;
using ClassifiedAds.Domain.Infrastructure.Messaging;
using ClassifiedAds.Infrastructure.AI;
using ClassifiedAds.Infrastructure.FeatureToggles.OutboxPublishingToggle;
using ClassifiedAds.Infrastructure.HostedServices;
using ClassifiedAds.Infrastructure.IdentityProviders.Auth0;
using ClassifiedAds.Infrastructure.IdentityProviders.Azure;
using ClassifiedAds.Infrastructure.Logging;
using ClassifiedAds.Infrastructure.Monitoring;
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

    services.AddMonitoringServices(appSettings.Monitoring);

    services.AddScoped<ICurrentUser, CurrentUser>();

    services.AddDateTimeProvider();
    services.AddPersistence(appSettings.ConnectionStrings.ClassifiedAds)
            .AddDomainServices()
            .AddApplicationServices()
            .AddMessageHandlers();

    services.AddStorageManager(appSettings.Storage);
    services.AddScoped<EmbeddingService>();
    services.AddScoped<ImageAnalysisService>();
    services.AddScoped<MarkdownService>();

    services.AddTransient<IMessageBus, MessageBus>();
    services.AddMessageBusSender<FileCreatedEvent>(appSettings.Messaging);
    services.AddMessageBusSender<FileUpdatedEvent>(appSettings.Messaging);
    services.AddMessageBusSender<FileDeletedEvent>(appSettings.Messaging);
    services.AddMessageBusSender<ProductCreatedEvent>(appSettings.Messaging);
    services.AddMessageBusSender<ProductUpdatedEvent>(appSettings.Messaging);
    services.AddMessageBusSender<ProductDeletedEvent>(appSettings.Messaging);
    services.AddMessageBusReceiver<WebhookConsumer, FileCreatedEvent>(appSettings.Messaging);
    services.AddMessageBusReceiver<WebhookConsumer, FileUpdatedEvent>(appSettings.Messaging);
    services.AddMessageBusReceiver<WebhookConsumer, FileDeletedEvent>(appSettings.Messaging);
    services.AddMessageBusReceiver<WebhookConsumer, ProductCreatedEvent>(appSettings.Messaging);
    services.AddMessageBusReceiver<WebhookConsumer, ProductUpdatedEvent>(appSettings.Messaging);
    services.AddMessageBusReceiver<WebhookConsumer, ProductDeletedEvent>(appSettings.Messaging);
    services.AddMessageBusReceiver<FileEmbeddingConsumer, FileCreatedEvent>(appSettings.Messaging);
    services.AddMessageBusReceiver<FileEmbeddingConsumer, FileUpdatedEvent>(appSettings.Messaging);
    services.AddMessageBusReceiver<FileEmbeddingConsumer, FileDeletedEvent>(appSettings.Messaging);
    services.AddMessageBusReceiver<ProductEmbeddingConsumer, ProductCreatedEvent>(appSettings.Messaging);
    services.AddMessageBusReceiver<ProductEmbeddingConsumer, ProductUpdatedEvent>(appSettings.Messaging);
    services.AddMessageBusReceiver<ProductEmbeddingConsumer, ProductDeletedEvent>(appSettings.Messaging);
    services.AddMessageBusConsumers(Assembly.GetExecutingAssembly());
    services.AddOutboxMessagePublishers(Assembly.GetExecutingAssembly());

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

    AddHealthChecks(services, appSettings);
    AddFeatureToggles(services);
    AddHostedServices(services);
})
.Build()
.Run();

static void AddFeatureToggles(IServiceCollection services)
{
    services.AddSingleton<IOutboxPublishingToggle, FileBasedOutboxPublishingToggle>();
}

static void AddHostedServices(IServiceCollection services)
{
    services.AddHostedService<MessageBusConsumerBackgroundService<WebhookConsumer, FileCreatedEvent>>();
    services.AddHostedService<MessageBusConsumerBackgroundService<WebhookConsumer, FileUpdatedEvent>>();
    services.AddHostedService<MessageBusConsumerBackgroundService<WebhookConsumer, FileDeletedEvent>>();
    services.AddHostedService<MessageBusConsumerBackgroundService<WebhookConsumer, ProductCreatedEvent>>();
    services.AddHostedService<MessageBusConsumerBackgroundService<WebhookConsumer, ProductUpdatedEvent>>();
    services.AddHostedService<MessageBusConsumerBackgroundService<WebhookConsumer, ProductDeletedEvent>>();
    services.AddHostedService<MessageBusConsumerBackgroundService<FileEmbeddingConsumer, FileCreatedEvent>>();
    services.AddHostedService<MessageBusConsumerBackgroundService<FileEmbeddingConsumer, FileUpdatedEvent>>();
    services.AddHostedService<MessageBusConsumerBackgroundService<FileEmbeddingConsumer, FileDeletedEvent>>();
    services.AddHostedService<MessageBusConsumerBackgroundService<ProductEmbeddingConsumer, ProductCreatedEvent>>();
    services.AddHostedService<MessageBusConsumerBackgroundService<ProductEmbeddingConsumer, ProductUpdatedEvent>>();
    services.AddHostedService<MessageBusConsumerBackgroundService<ProductEmbeddingConsumer, ProductDeletedEvent>>();
    services.AddHostedService<PublishOutboxWorker>();
    services.AddHostedService<SendEmailWorker>();
    services.AddHostedService<SendSmsWorker>();
    services.AddHostedService<ScheduleCronJobWorker>();
    services.AddHostedService<SyncUsersWorker>();
}

static void AddHealthChecks(IServiceCollection services, AppSettings appSettings)
{
    services.AddHealthChecks()
        .AddSqlServer(connectionString: appSettings.ConnectionStrings.ClassifiedAds,
            healthQuery: "SELECT 1;",
            name: "Sql Server",
            failureStatus: HealthStatus.Degraded)
        .AddMessageBusHealthCheck(appSettings.Messaging);

    services.Configure<HealthChecksBackgroundServiceOptions>(x => x.Interval = TimeSpan.FromMinutes(10));
    services.AddHostedService<HealthChecksBackgroundService>();
}