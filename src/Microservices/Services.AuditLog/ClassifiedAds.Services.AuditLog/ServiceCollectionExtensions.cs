using ClassifiedAds.Domain.Infrastructure.Messaging;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Infrastructure.HostedServices;
using ClassifiedAds.Services.AuditLog.ConfigurationOptions;
using ClassifiedAds.Services.AuditLog.DTOs;
using ClassifiedAds.Services.AuditLog.Entities;
using ClassifiedAds.Services.AuditLog.MessageBusConsumers;
using ClassifiedAds.Services.AuditLog.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuditLogModule(this IServiceCollection services, AppSettings appSettings)
    {
        services.AddCaches(appSettings.Caching);

        services.AddDbContext<AuditLogDbContext>(options => options.UseSqlServer(appSettings.ConnectionStrings.ClassifiedAds, sql =>
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
            .AddScoped<IRepository<AuditLogEntry, Guid>, Repository<AuditLogEntry, Guid>>()
            .AddScoped<IRepository<IdempotentRequest, Guid>, Repository<IdempotentRequest, Guid>>();

        services.AddMessageHandlers(Assembly.GetExecutingAssembly());

        services.AddAuthorizationPolicies(Assembly.GetExecutingAssembly());

        services.AddTransient<IMessageBus, MessageBus>()
                .AddMessageBusReceiver<AuditLogAggregationConsumer, AuditLogCreatedEvent>(appSettings.Messaging);

        return services;
    }

    public static void MigrateAuditLogDb(this IApplicationBuilder app)
    {
        using (var serviceScope = app.ApplicationServices.CreateScope())
        {
            serviceScope.ServiceProvider.GetRequiredService<AuditLogDbContext>().Database.Migrate();
        }
    }

    public static IServiceCollection AddHostedServicesAuditLogModule(this IServiceCollection services)
    {
        services.AddMessageBusConsumers(Assembly.GetExecutingAssembly());
        services.AddOutboxMessagePublishers(Assembly.GetExecutingAssembly());

        services.AddHostedService<MessageBusConsumerBackgroundService<AuditLogAggregationConsumer, AuditLogCreatedEvent>>();

        return services;
    }
}
