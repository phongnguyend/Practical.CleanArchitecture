using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Modules.Storage.Authorization;
using ClassifiedAds.Modules.Storage.ConfigurationOptions;
using ClassifiedAds.Modules.Storage.Entities;
using ClassifiedAds.Modules.Storage.HostedServices;
using ClassifiedAds.Modules.Storage.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class StorageModuleServiceCollectionExtensions
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
        }))
            .AddScoped<IRepository<FileEntry, Guid>, Repository<FileEntry, Guid>>()
            .AddScoped<IRepository<AuditLogEntry, Guid>, Repository<AuditLogEntry, Guid>>()
            .AddScoped<IRepository<OutboxEvent, Guid>, Repository<OutboxEvent, Guid>>();

        services.AddMessageHandlers(Assembly.GetExecutingAssembly());

        services.AddStorageManager(settings);

        services.AddAuthorizationPolicies(Assembly.GetExecutingAssembly(), AuthorizationPolicyNames.GetPolicyNames());

        return services;
    }

    public static IMvcBuilder AddStorageModule(this IMvcBuilder builder)
    {
        return builder.AddApplicationPart(Assembly.GetExecutingAssembly());
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
        services.AddHostedService<MessageBusReceiver>();
        services.AddHostedService<PublishEventWorker>();

        return services;
    }
}
