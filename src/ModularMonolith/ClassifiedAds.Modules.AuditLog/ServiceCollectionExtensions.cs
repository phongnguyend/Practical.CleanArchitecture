using ClassifiedAds.Contracts.AuditLog.Services;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Modules.AuditLog.ConfigurationOptions;
using ClassifiedAds.Modules.AuditLog.Entities;
using ClassifiedAds.Modules.AuditLog.Persistence;
using ClassifiedAds.Modules.AuditLog.RateLimiterPolicies;
using ClassifiedAds.Modules.AuditLog.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;
using RateLimiterPolicyNames = ClassifiedAds.Modules.AuditLog.RateLimiterPolicies.RateLimiterPolicyNames;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuditLogModule(this IServiceCollection services, Action<AuditLogModuleOptions> configureOptions)
    {
        var settings = new AuditLogModuleOptions();
        configureOptions(settings);

        services.Configure(configureOptions);

        services.AddDbContext<AuditLogDbContext>(options => options.UseSqlServer(settings.ConnectionStrings.Default, sql =>
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
            .AddScoped<IRepository<AuditLogEntry, Guid>, Repository<AuditLogEntry, Guid>>()
            .AddScoped<IRepository<IdempotentRequest, Guid>, Repository<IdempotentRequest, Guid>>()
            .AddScoped(typeof(IAuditLogService), typeof(AuditLogService));

        services.AddMessageHandlers(Assembly.GetExecutingAssembly());

        services.AddAuthorizationPolicies(Assembly.GetExecutingAssembly());

        services.AddRateLimiter(options =>
        {
            options.AddPolicy<string, GetAuditLogsRateLimiterPolicy>(RateLimiterPolicyNames.GetAuditLogsPolicy);
        });

        return services;
    }

    public static IMvcBuilder AddAuditLogModule(this IMvcBuilder builder)
    {
        return builder.AddApplicationPart(Assembly.GetExecutingAssembly());
    }

    public static void MigrateAuditLogDb(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        serviceScope.ServiceProvider.GetRequiredService<AuditLogDbContext>().Database.Migrate();
    }

    public static void MigrateAuditLogDb(this IHost app)
    {
        using var serviceScope = app.Services.CreateScope();
        serviceScope.ServiceProvider.GetRequiredService<AuditLogDbContext>().Database.Migrate();
    }
}
