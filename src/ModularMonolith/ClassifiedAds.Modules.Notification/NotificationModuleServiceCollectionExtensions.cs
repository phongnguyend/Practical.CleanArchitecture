using ClassifiedAds.Domain.Events;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Infrastructure.MessageBrokers;
using ClassifiedAds.Infrastructure.Notification;
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
        public static IServiceCollection AddNotificationModule(this IServiceCollection services, MessageBrokerOptions messageBrokerOptions, NotificationOptions notificationOptions, string connectionString, string migrationsAssembly = "")
        {
            services
                .AddDbContext<NotificationDbContext>(options => options.UseSqlServer(connectionString, sql =>
                {
                    if (!string.IsNullOrEmpty(migrationsAssembly))
                    {
                        sql.MigrationsAssembly(migrationsAssembly);
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
                .AddMessageBusSender<EmailMessageCreatedEvent>(messageBrokerOptions)
                .AddMessageBusSender<SmsMessageCreatedEvent>(messageBrokerOptions);

            services.AddNotificationServices(notificationOptions);

            return services;
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
