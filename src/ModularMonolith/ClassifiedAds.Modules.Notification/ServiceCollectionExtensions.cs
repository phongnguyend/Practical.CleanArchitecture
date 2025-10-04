using ClassifiedAds.Contracts.Notification.Services;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Modules.Notification.ConfigurationOptions;
using ClassifiedAds.Modules.Notification.Entities;
using ClassifiedAds.Modules.Notification.HostedServices;
using ClassifiedAds.Modules.Notification.Persistence;
using ClassifiedAds.Modules.Notification.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNotificationModule(this IServiceCollection services, Action<NotificationModuleOptions> configureOptions)
    {
        var settings = new NotificationModuleOptions();
        configureOptions(settings);

        services.Configure(configureOptions);

        services
            .AddDbContext<NotificationDbContext>(options => options.UseSqlServer(settings.ConnectionStrings.Default, sql =>
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
            .AddScoped<IRepository<EmailMessage, Guid>, Repository<EmailMessage, Guid>>()
            .AddScoped<IRepository<SmsMessage, Guid>, Repository<SmsMessage, Guid>>()
            .AddScoped(typeof(IEmailMessageRepository), typeof(EmailMessageRepository))
            .AddScoped(typeof(ISmsMessageRepository), typeof(SmsMessageRepository))
            .AddScoped<IEmailMessageService, EmailMessageService>();

        services.AddMessageHandlers(Assembly.GetExecutingAssembly());

        services.AddNotificationServices(settings);

        return services;
    }

    public static IMvcBuilder AddNotificationModule(this IMvcBuilder builder)
    {
        return builder.AddApplicationPart(Assembly.GetExecutingAssembly());
    }

    public static void MigrateNotificationDb(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        serviceScope.ServiceProvider.GetRequiredService<NotificationDbContext>().Database.Migrate();
    }

    public static void MigrateNotificationDb(this IHost app)
    {
        using var serviceScope = app.Services.CreateScope();
        serviceScope.ServiceProvider.GetRequiredService<NotificationDbContext>().Database.Migrate();
    }

    public static IServiceCollection AddHostedServicesNotificationModule(this IServiceCollection services)
    {
        services.AddHostedService<SendEmailWorker>();
        services.AddHostedService<SendSmsWorker>();

        return services;
    }
}
