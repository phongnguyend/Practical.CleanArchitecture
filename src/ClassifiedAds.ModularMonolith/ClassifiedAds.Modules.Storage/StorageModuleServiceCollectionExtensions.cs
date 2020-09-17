using ClassifiedAds.Domain.Events;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Infrastructure.MessageBrokers;
using ClassifiedAds.Modules.Storage;
using ClassifiedAds.Modules.Storage.DTOs;
using ClassifiedAds.Modules.Storage.Entities;
using ClassifiedAds.Modules.Storage.Repositories;
using ClassifiedAds.Modules.Storage.Storages;
using ClassifiedAds.Modules.Storage.Storages.Amazon;
using ClassifiedAds.Modules.Storage.Storages.Azure;
using ClassifiedAds.Modules.Storage.Storages.Fake;
using ClassifiedAds.Modules.Storage.Storages.Local;
using Microsoft.AspNetCore.Builder;
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

            if (storageOptions.UsedAzure())
            {
                services.AddSingleton<IFileStorageManager>(new AzureBlobStorageManager(storageOptions.Azure.ConnectionString, storageOptions.Azure.Container));
            }
            else if (storageOptions.UsedAmazon())
            {
                services.AddSingleton<IFileStorageManager>(
                    new AmazonS3StorageManager(
                        storageOptions.Amazon.AccessKeyID,
                        storageOptions.Amazon.SecretAccessKey,
                        storageOptions.Amazon.BucketName,
                        storageOptions.Amazon.RegionEndpoint));
            }
            else if (storageOptions.UsedLocal())
            {
                services.AddSingleton<IFileStorageManager>(new LocalFileStorageManager(storageOptions.Local.Path));
            }
            else if (storageOptions.UsedFake())
            {
                services.AddSingleton<IFileStorageManager>(new FakeStorageManager());
            }

            services.AddMessageBusSender<FileUploadedEvent>(messageBrokerOptions);
            services.AddMessageBusSender<FileDeletedEvent>(messageBrokerOptions);

            return services;
        }

        public static void MigrateDb(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<StorageDbContext>().Database.Migrate();
            }
        }
    }
}
