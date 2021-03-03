using ClassifiedAds.Domain.Events;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Modules.Storage.ConfigurationOptions;
using ClassifiedAds.Modules.Storage.DTOs;
using ClassifiedAds.Modules.Storage.Entities;
using ClassifiedAds.Modules.Storage.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class StorageModuleServiceCollectionExtensions
    {
        public static IServiceCollection AddStorageModule(this IServiceCollection services, StorageModuleOptions moduleOptions)
        {
            services.Configure<StorageModuleOptions>(op =>
            {
                op.ConnectionStrings = moduleOptions.ConnectionStrings;
                op.Storage = moduleOptions.Storage;
                op.MessageBroker = moduleOptions.MessageBroker;
            });

            services.AddDbContext<StorageDbContext>(options => options.UseSqlServer(moduleOptions.ConnectionStrings.Default, sql =>
            {
                if (!string.IsNullOrEmpty(moduleOptions.ConnectionStrings.MigrationsAssembly))
                {
                    sql.MigrationsAssembly(moduleOptions.ConnectionStrings.MigrationsAssembly);
                }
            }))
                .AddScoped<IRepository<FileEntry, Guid>, Repository<FileEntry, Guid>>();

            DomainEvents.RegisterHandlers(Assembly.GetExecutingAssembly(), services);

            services.AddMessageHandlers(Assembly.GetExecutingAssembly());

            services.AddStorageManager(moduleOptions.Storage);

            services.AddMessageBusSender<FileUploadedEvent>(moduleOptions.MessageBroker);
            services.AddMessageBusSender<FileDeletedEvent>(moduleOptions.MessageBroker);

            services.AddAuthorizationPolicies(Assembly.GetExecutingAssembly());

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
    }
}
