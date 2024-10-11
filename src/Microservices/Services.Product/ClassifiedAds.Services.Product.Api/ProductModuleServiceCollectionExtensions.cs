using ClassifiedAds.CrossCuttingConcerns.Csv;
using ClassifiedAds.CrossCuttingConcerns.Html;
using ClassifiedAds.CrossCuttingConcerns.Pdf;
using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Infrastructure.Identity;
using ClassifiedAds.Services.Product.Authorization;
using ClassifiedAds.Services.Product.ConfigurationOptions;
using ClassifiedAds.Services.Product.Csv;
using ClassifiedAds.Services.Product.DTOs;
using ClassifiedAds.Services.Product.Entities;
using ClassifiedAds.Services.Product.HostedServices;
using ClassifiedAds.Services.Product.Html;
using ClassifiedAds.Services.Product.Pdf;
using ClassifiedAds.Services.Product.Pdf.DinkToPdf;
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

        services.AddScoped<ICsvReader<ImportProductsFromCsv>, ImportProductsFromCsvHandler>();
        services.AddScoped<ICsvWriter<ExportProductsToCsv>, ExportProductsToCsvHandler>();

        services.AddScoped<IHtmlWriter<ExportProductsToHtml>, ExportProductsToHtmlHandler>();

        services.AddScoped<IPdfWriter<ExportProductsToPdf>, ExportProductsToPdfHandler>();

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
