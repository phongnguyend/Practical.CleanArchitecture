using ClassifiedAds.Domain.Events;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Infrastructure.Identity;
using ClassifiedAds.Infrastructure.MessageBrokers;
using ClassifiedAds.Infrastructure.Storages;
using ClassifiedAds.Services.Storage.DTOs;
using ClassifiedAds.Services.Storage.Entities;
using ClassifiedAds.Services.Storage.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class StorageModuleServiceCollectionExtensions
    {
        public static IServiceCollection AddStorageModule(this IServiceCollection services, StorageOptions storageOptions, MessageBrokerOptions messageBrokerOptions, string connectionString, string migrationsAssembly = "")
        {
            services.AddDbContext<StorageDbContext>(options => options.UseSqlServer(connectionString, sql =>
            {
                if (!string.IsNullOrEmpty(migrationsAssembly))
                {
                    sql.MigrationsAssembly(migrationsAssembly);
                }
            }))
                .AddScoped<IRepository<FileEntry, Guid>, Repository<FileEntry, Guid>>();

            DomainEvents.RegisterHandlers(Assembly.GetExecutingAssembly(), services);

            services.AddMessageHandlers(Assembly.GetExecutingAssembly());

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<ICurrentUser, CurrentWebUser>();

            services.AddStorageManager(storageOptions);

            services.AddMessageBusSender<FileUploadedEvent>(messageBrokerOptions)
                    .AddMessageBusSender<FileDeletedEvent>(messageBrokerOptions)
                    .AddMessageBusReceiver<FileUploadedEvent>(messageBrokerOptions)
                    .AddMessageBusReceiver<FileDeletedEvent>(messageBrokerOptions);

            return services;
        }

        public static void MigrateStorageDb(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<StorageDbContext>().Database.Migrate();
            }
        }
    }
}
