using ClassifiedAds.BackgroundServer.ConfigurationOptions;
using ClassifiedAds.BackgroundServer.Identity;
using ClassifiedAds.Contracts.Identity.Services;
using ClassifiedAds.Infrastructure.Logging;
using ClassifiedAds.Modules.Storage.DTOs;
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
        .AddNotificationModule(opt => configuration.GetSection("Modules:Notification").Bind(opt))
        .AddProductModule(opt => configuration.GetSection("Modules:Product").Bind(opt))
        .AddStorageModule(opt => configuration.GetSection("Modules:Storage").Bind(opt))
        .AddApplicationServices();

        services.AddMessageBusSender<FileUploadedEvent>(appSettings.MessageBroker);
        services.AddMessageBusSender<FileDeletedEvent>(appSettings.MessageBroker);

        services.AddMessageBusReceiver<FileUploadedEvent>(appSettings.MessageBroker);
        services.AddMessageBusReceiver<FileDeletedEvent>(appSettings.MessageBroker);

        services.AddHostedServicesNotificationModule();
        services.AddHostedServicesProductModule();
        services.AddHostedServicesStorageModule();
    });