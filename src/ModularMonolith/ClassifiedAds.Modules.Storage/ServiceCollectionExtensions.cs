using ClassifiedAds.Domain.Infrastructure.Messaging;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Infrastructure.HostedServices;
using ClassifiedAds.Modules.Storage.ConfigurationOptions;
using ClassifiedAds.Modules.Storage.DTOs;
using ClassifiedAds.Modules.Storage.Entities;
using ClassifiedAds.Modules.Storage.HostedServices;
using ClassifiedAds.Modules.Storage.MessageBusConsumers;
using ClassifiedAds.Modules.Storage.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStorageModule(this IServiceCollection services, Action<StorageModuleOptions> configureOptions)
    {
        var settings = new StorageModuleOptions();
        configureOptions(settings);

        services.Configure(configureOptions);

        services.AddDbContext<StorageDbContext>(options => options.UseSqlServer(settings.ConnectionStrings.Default, sql =>
        {
            if (!string.IsNullOrEmpty(settings.ConnectionStrings.MigrationsAssembly))
            {
                sql.MigrationsAssembly(settings.ConnectionStrings.MigrationsAssembly);
            }

            if (settings.ConnectionStrings.CommandTimeout.HasValue)
            {
                sql.CommandTimeout(settings.ConnectionStrings.CommandTimeout);
            }
        }))
            .AddScoped<IRepository<FileEntry, Guid>, Repository<FileEntry, Guid>>()
            .AddScoped<IRepository<AuditLogEntry, Guid>, Repository<AuditLogEntry, Guid>>()
            .AddScoped<IRepository<OutboxMessage, Guid>, Repository<OutboxMessage, Guid>>();

        services.AddMessageHandlers(Assembly.GetExecutingAssembly());

        services.AddStorageManager(settings);

        services.AddAuthorizationPolicies(Assembly.GetExecutingAssembly());

        return services;
    }

    public static IMvcBuilder AddStorageModule(this IMvcBuilder builder)
    {
        return builder.AddApplicationPart(Assembly.GetExecutingAssembly());
    }

    public static void MigrateStorageDb(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        serviceScope.ServiceProvider.GetRequiredService<StorageDbContext>().Database.Migrate();
    }

    public static void MigrateStorageDb(this IHost app)
    {
        using var serviceScope = app.Services.CreateScope();
        serviceScope.ServiceProvider.GetRequiredService<StorageDbContext>().Database.Migrate();
    }

    public static IServiceCollection AddHostedServicesStorageModule(this IServiceCollection services)
    {
        services.AddMessageBusConsumers(Assembly.GetExecutingAssembly());
        services.AddOutboxMessagePublishers(Assembly.GetExecutingAssembly());

        services.AddHostedService<MessageBusConsumerBackgroundService<WebhookConsumer, FileUploadedEvent>>();
        services.AddHostedService<MessageBusConsumerBackgroundService<WebhookConsumer, FileDeletedEvent>>();
        services.AddHostedService<PublishEventWorker>();

        return services;
    }
}
