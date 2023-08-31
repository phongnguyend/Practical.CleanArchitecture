using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Services.AuditLog.Authorization;
using ClassifiedAds.Services.AuditLog.ConfigurationOptions;
using ClassifiedAds.Services.AuditLog.DTOs;
using ClassifiedAds.Services.AuditLog.Entities;
using ClassifiedAds.Services.AuditLog.HostedServices;
using ClassifiedAds.Services.AuditLog.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class AuditLogModuleServiceCollectionExtensions
{
    public static IServiceCollection AddAuditLogModule(this IServiceCollection services, AppSettings appSettings)
    {
        services.AddDbContext<AuditLogDbContext>(options => options.UseSqlServer(appSettings.ConnectionStrings.ClassifiedAds, sql =>
        {
            if (!string.IsNullOrEmpty(appSettings.ConnectionStrings.MigrationsAssembly))
            {
                sql.MigrationsAssembly(appSettings.ConnectionStrings.MigrationsAssembly);
            }
        }))
            .AddScoped<IRepository<AuditLogEntry, Guid>, Repository<AuditLogEntry, Guid>>()
            .AddScoped<IRepository<IdempotentRequest, Guid>, Repository<IdempotentRequest, Guid>>();

        services.AddMessageHandlers(Assembly.GetExecutingAssembly());

        services.AddAuthorizationPolicies(Assembly.GetExecutingAssembly(), AuthorizationPolicyNames.GetPolicyNames());

        services.AddMessageBusReceiver<AuditLogAggregationConsumer, AuditLogCreatedEvent>(appSettings.MessageBroker);

        return services;
    }

    public static void MigrateAuditLogDb(this IApplicationBuilder app)
    {
        using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            serviceScope.ServiceProvider.GetRequiredService<AuditLogDbContext>().Database.Migrate();
        }
    }

    public static IServiceCollection AddHostedServicesAuditLogModule(this IServiceCollection services)
    {
        services.AddHostedService<MessageBusReceiver>();

        return services;
    }
}
