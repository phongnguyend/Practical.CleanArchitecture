using ClassifiedAds.BackgroundServer.ConfigurationOptions;
using ClassifiedAds.BackgroundServer.Identity;
using ClassifiedAds.Contracts.Identity.Services;
using ClassifiedAds.Infrastructure.Logging;
using ClassifiedAds.Modules.Identity.Repositories;
using ClassifiedAds.Modules.Storage.DTOs;
using ClassifiedAds.Modules.Storage.HostedServices;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

CreateHostBuilder(args).Build().Run();

static IHostBuilder CreateHostBuilder(string[] args) =>
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

        services
        .AddAuditLogModule(opt => configuration.GetSection("Modules:AuditLog").Bind(opt))
        .AddIdentityModuleCore(opt => configuration.GetSection("Modules:Identity").Bind(opt))
        .AddNotificationModule(opt => configuration.GetSection("Modules:Notification").Bind(opt))
        .AddProductModule(opt => configuration.GetSection("Modules:Product").Bind(opt))
        .AddStorageModule(opt => configuration.GetSection("Modules:Storage").Bind(opt))
        .AddApplicationServices();

        services.AddDataProtection()
        .PersistKeysToDbContext<IdentityDbContext>()
        .SetApplicationName("ClassifiedAds");

        services.AddMessageBusSender<FileUploadedEvent>(appSettings.MessageBroker);
        services.AddMessageBusSender<FileDeletedEvent>(appSettings.MessageBroker);

        services.AddMessageBusReceiver<WebhookConsumer, FileUploadedEvent>(appSettings.MessageBroker);
        services.AddMessageBusReceiver<WebhookConsumer, FileDeletedEvent>(appSettings.MessageBroker);

        services.AddHostedServicesIdentityModule();
        services.AddHostedServicesNotificationModule();
        services.AddHostedServicesProductModule();
        services.AddHostedServicesStorageModule();
    });