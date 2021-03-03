using ClassifiedAds.Domain.Events;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Modules.Notification.ConfigurationOptions;
using ClassifiedAds.Modules.Notification.Contracts.DTOs;
using ClassifiedAds.Modules.Notification.Contracts.Services;
using ClassifiedAds.Modules.Notification.Entities;
using ClassifiedAds.Modules.Notification.Repositories;
using ClassifiedAds.Modules.Notification.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class NotificationModuleServiceCollectionExtensions
    {
        public static IServiceCollection AddNotificationModule(this IServiceCollection services, NotificationModuleOptions moduleOptions)
        {
            services.Configure<NotificationModuleOptions>(op =>
            {
                op.ConnectionStrings = moduleOptions.ConnectionStrings;
                op.MessageBroker = moduleOptions.MessageBroker;
                op.Notification = moduleOptions.Notification;
            });

            services
                .AddDbContext<NotificationDbContext>(options => options.UseSqlServer(moduleOptions.ConnectionStrings.Default, sql =>
                {
                    if (!string.IsNullOrEmpty(moduleOptions.ConnectionStrings.MigrationsAssembly))
                    {
                        sql.MigrationsAssembly(moduleOptions.ConnectionStrings.MigrationsAssembly);
                    }
                }))
                .AddScoped<IRepository<EmailMessage, Guid>, Repository<EmailMessage, Guid>>()
                .AddScoped<IRepository<SmsMessage, Guid>, Repository<SmsMessage, Guid>>()
                .AddScoped(typeof(IEmailMessageRepository), typeof(EmailMessageRepository))
                .AddScoped(typeof(ISmsMessageRepository), typeof(SmsMessageRepository))
                .AddScoped<IEmailMessageService, EmailMessageService>();

            services
                .AddScoped<EmailMessageService>()
                .AddScoped<SmsMessageService>();

            DomainEvents.RegisterHandlers(Assembly.GetExecutingAssembly(), services);

            services.AddMessageHandlers(Assembly.GetExecutingAssembly());

            services
                .AddMessageBusSender<EmailMessageCreatedEvent>(moduleOptions.MessageBroker)
                .AddMessageBusSender<SmsMessageCreatedEvent>(moduleOptions.MessageBroker);

            services.AddNotificationServices(moduleOptions.Notification);

            return services;
        }

        public static IMvcBuilder AddNotificationModule(this IMvcBuilder builder)
        {
            return builder.AddApplicationPart(Assembly.GetExecutingAssembly());
        }

        public static void MigrateNotificationDb(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<NotificationDbContext>().Database.Migrate();
            }
        }
    }
}
