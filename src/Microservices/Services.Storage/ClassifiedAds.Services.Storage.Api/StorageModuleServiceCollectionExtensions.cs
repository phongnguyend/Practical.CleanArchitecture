using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Infrastructure.HostedServices;
using ClassifiedAds.Infrastructure.Identity;
using ClassifiedAds.Services.Storage.Authorization;
using ClassifiedAds.Services.Storage.ConfigurationOptions;
using ClassifiedAds.Services.Storage.DTOs;
using ClassifiedAds.Services.Storage.Entities;
using ClassifiedAds.Services.Storage.HostedServices;
using ClassifiedAds.Services.Storage.MessageBusConsumers;
using ClassifiedAds.Services.Storage.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class StorageModuleServiceCollectionExtensions
{
    public static IServiceCollection AddStorageModule(this IServiceCollection services, AppSettings appSettings)
    {
        services.AddDbContext<StorageDbContext>(options => options.UseSqlServer(appSettings.ConnectionStrings.ClassifiedAds, sql =>
        {
            if (!string.IsNullOrEmpty(appSettings.ConnectionStrings.MigrationsAssembly))
            {
                sql.MigrationsAssembly(appSettings.ConnectionStrings.MigrationsAssembly);
            }
        }))
            .AddScoped<IRepository<FileEntry, Guid>, Repository<FileEntry, Guid>>()
            .AddScoped<IRepository<AuditLogEntry, Guid>, Repository<AuditLogEntry, Guid>>()
            .AddScoped<IRepository<OutboxEvent, Guid>, Repository<OutboxEvent, Guid>>();

        services.AddMessageHandlers(Assembly.GetExecutingAssembly());

        services.AddAuthorizationPolicies(Assembly.GetExecutingAssembly(), AuthorizationPolicyNames.GetPolicyNames());

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<ICurrentUser, CurrentWebUser>();

        services.AddStorageManager(appSettings.Storage);

        services.AddTransient<IMessageBus, MessageBus>()
                .AddMessageBusSender<FileUploadedEvent>(appSettings.MessageBroker)
                .AddMessageBusSender<FileDeletedEvent>(appSettings.MessageBroker)
                .AddMessageBusSender<AuditLogCreatedEvent>(appSettings.MessageBroker)
                .AddMessageBusReceiver<WebhookConsumer, FileUploadedEvent>(appSettings.MessageBroker)
                .AddMessageBusReceiver<WebhookConsumer, FileDeletedEvent>(appSettings.MessageBroker);

        return services;
    }

    public static void MigrateStorageDb(this IApplicationBuilder app)
    {
        using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
        {
            serviceScope.ServiceProvider.GetRequiredService<StorageDbContext>().Database.Migrate();
        }
    }

    public static IServiceCollection AddHostedServicesStorageModule(this IServiceCollection services)
    {
        services.AddMessageBusConsumers(Assembly.GetExecutingAssembly());
        services.AddOutboxEventPublishers(Assembly.GetExecutingAssembly());

        services.AddHostedService<MessageBusConsumerBackgroundService<WebhookConsumer, FileUploadedEvent>>();
        services.AddHostedService<MessageBusConsumerBackgroundService<WebhookConsumer, FileDeletedEvent>>();
        services.AddHostedService<PublishEventWorker>();

        return services;
    }
}
