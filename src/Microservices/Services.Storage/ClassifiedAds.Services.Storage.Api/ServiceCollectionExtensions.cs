using ClassifiedAds.Domain.Infrastructure.Messaging;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Infrastructure.HostedServices;
using ClassifiedAds.Infrastructure.Identity;
using ClassifiedAds.Services.Storage.ConfigurationOptions;
using ClassifiedAds.Services.Storage.DTOs;
using ClassifiedAds.Services.Storage.Entities;
using ClassifiedAds.Services.Storage.HostedServices;
using ClassifiedAds.Services.Storage.MessageBusConsumers;
using ClassifiedAds.Services.Storage.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStorageModule(this IServiceCollection services, AppSettings appSettings)
    {
        services.AddCaches(appSettings.Caching);

        services.AddDbContext<StorageDbContext>(options => options.UseSqlServer(appSettings.ConnectionStrings.ClassifiedAds, sql =>
        {
            if (!string.IsNullOrEmpty(appSettings.ConnectionStrings.MigrationsAssembly))
            {
                sql.MigrationsAssembly(appSettings.ConnectionStrings.MigrationsAssembly);
            }

            if (appSettings.ConnectionStrings.CommandTimeout.HasValue)
            {
                sql.CommandTimeout(appSettings.ConnectionStrings.CommandTimeout);
            }
        }))
            .AddScoped<IRepository<FileEntry, Guid>, Repository<FileEntry, Guid>>()
            .AddScoped<IRepository<AuditLogEntry, Guid>, Repository<AuditLogEntry, Guid>>()
            .AddScoped<IRepository<OutboxMessage, Guid>, Repository<OutboxMessage, Guid>>();

        services.AddMessageHandlers(Assembly.GetExecutingAssembly());

        services.AddAuthorizationPolicies(Assembly.GetExecutingAssembly());

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<ICurrentUser, CurrentWebUser>();

        services.AddStorageManager(appSettings.Storage);

        services.AddTransient<IMessageBus, MessageBus>()
                .AddMessageBusSender<FileUploadedEvent>(appSettings.Messaging)
                .AddMessageBusSender<FileDeletedEvent>(appSettings.Messaging)
                .AddMessageBusSender<AuditLogCreatedEvent>(appSettings.Messaging)
                .AddMessageBusReceiver<WebhookConsumer, FileUploadedEvent>(appSettings.Messaging)
                .AddMessageBusReceiver<WebhookConsumer, FileDeletedEvent>(appSettings.Messaging);

        return services;
    }

    public static void MigrateStorageDb(this IApplicationBuilder app)
    {
        using (var serviceScope = app.ApplicationServices.CreateScope())
        {
            serviceScope.ServiceProvider.GetRequiredService<StorageDbContext>().Database.Migrate();
        }
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
