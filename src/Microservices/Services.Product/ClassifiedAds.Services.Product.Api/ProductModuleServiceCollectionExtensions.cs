using ClassifiedAds.CrossCuttingConcerns.Csv;
using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Infrastructure.Csv;
using ClassifiedAds.Infrastructure.Identity;
using ClassifiedAds.Services.Product.Authorization;
using ClassifiedAds.Services.Product.ConfigurationOptions;
using ClassifiedAds.Services.Product.DTOs;
using ClassifiedAds.Services.Product.Entities;
using ClassifiedAds.Services.Product.HostedServices;
using ClassifiedAds.Services.Product.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class ProductModuleServiceCollectionExtensions
{
    public static IServiceCollection AddProductModule(this IServiceCollection services, AppSettings appSettings)
    {
        services.AddDbContext<ProductDbContext>(options => options.UseSqlServer(appSettings.ConnectionStrings.ClassifiedAds, sql =>
        {
            if (!string.IsNullOrEmpty(appSettings.ConnectionStrings.MigrationsAssembly))
            {
                sql.MigrationsAssembly(appSettings.ConnectionStrings.MigrationsAssembly);
            }
        }));

        services
            .AddScoped<IRepository<Product, Guid>, Repository<Product, Guid>>()
            .AddScoped(typeof(IProductRepository), typeof(ProductRepository))
            .AddScoped<IRepository<AuditLogEntry, Guid>, Repository<AuditLogEntry, Guid>>()
            .AddScoped<IRepository<OutboxEvent, Guid>, Repository<OutboxEvent, Guid>>();

        services.AddMessageHandlers(Assembly.GetExecutingAssembly());

        services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddAuthorizationPolicies(Assembly.GetExecutingAssembly(), AuthorizationPolicyNames.GetPolicyNames());

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<ICurrentUser, CurrentWebUser>();

        services.AddScoped(typeof(ICsvReader<>), typeof(CsvReader<>));
        services.AddScoped(typeof(ICsvWriter<>), typeof(CsvWriter<>));

        services.AddTransient<IMessageBus, MessageBus>()
                .AddMessageBusSender<AuditLogCreatedEvent>(appSettings.MessageBroker);

        return services;
    }

    public static void MigrateProductDb(this IApplicationBuilder app)
    {
        using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
        {
            serviceScope.ServiceProvider.GetRequiredService<ProductDbContext>().Database.Migrate();
        }
    }

    public static IServiceCollection AddHostedServicesProductModule(this IServiceCollection services)
    {
        services.AddMessageBusConsumers(Assembly.GetExecutingAssembly());
        services.AddOutboxEventPublishers(Assembly.GetExecutingAssembly());

        services.AddHostedService<PublishEventWorker>();

        return services;
    }
}
