using ClassifiedAds.Application.FeatureToggles;
using ClassifiedAds.Background.ConfigurationOptions;
using ClassifiedAds.Background.Identity;
using ClassifiedAds.Contracts.Identity.Services;
using ClassifiedAds.Domain.Infrastructure.Messaging;
using ClassifiedAds.Infrastructure.FeatureToggles.OutboxPublishingToggle;
using ClassifiedAds.Infrastructure.Logging;
using ClassifiedAds.Infrastructure.Monitoring;
using ClassifiedAds.Modules.Identity.Persistence;
using ClassifiedAds.Modules.Storage.DTOs;
using ClassifiedAds.Modules.Storage.MessageBusConsumers;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

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

    services.AddMonitoringServices(appSettings.Monitoring);

    services.AddScoped<ICurrentUser, CurrentUser>();

    services.AddDateTimeProvider();

    services.AddCaches(appSettings.Caching);

    services
    .AddAuditLogModule(opt => configuration.GetSection("Modules:AuditLog").Bind(opt))
    .AddIdentityModuleCore(opt => configuration.GetSection("Modules:Identity").Bind(opt))
    .AddNotificationModule(opt => configuration.GetSection("Modules:Notification").Bind(opt))
    .AddProductModule(opt => configuration.GetSection("Modules:Product").Bind(opt))
    .AddStorageModule(opt => configuration.GetSection("Modules:Storage").Bind(opt))
    .AddApplicationServices();

    services.AddHtmlRazorLightEngine();
    services.AddDinkToPdfConverter();

    services.AddDataProtection()
    .PersistKeysToDbContext<IdentityDbContext>()
    .SetApplicationName("ClassifiedAds");

    services.AddTransient<IMessageBus, MessageBus>();
    services.AddMessageBusSender<FileUploadedEvent>(appSettings.Messaging);
    services.AddMessageBusSender<FileDeletedEvent>(appSettings.Messaging);
    services.AddMessageBusReceiver<WebhookConsumer, FileUploadedEvent>(appSettings.Messaging);
    services.AddMessageBusReceiver<WebhookConsumer, FileDeletedEvent>(appSettings.Messaging);

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
    services.AddHostedServicesIdentityModule();
    services.AddHostedServicesNotificationModule();
    services.AddHostedServicesProductModule();
    services.AddHostedServicesStorageModule();
}