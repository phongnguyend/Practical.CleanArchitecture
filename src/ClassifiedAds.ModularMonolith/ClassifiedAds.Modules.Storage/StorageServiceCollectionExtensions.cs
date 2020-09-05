using ClassifiedAds.Domain.Events;
using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Infrastructure.MessageBrokers.AzureEventGrid;
using ClassifiedAds.Infrastructure.MessageBrokers.AzureEventHub;
using ClassifiedAds.Infrastructure.MessageBrokers.AzureQueue;
using ClassifiedAds.Infrastructure.MessageBrokers.AzureServiceBus;
using ClassifiedAds.Infrastructure.MessageBrokers.Kafka;
using ClassifiedAds.Infrastructure.MessageBrokers.RabbitMQ;
using ClassifiedAds.Modules.Storage;
using ClassifiedAds.Modules.Storage.ConfigurationOptions.MessageBroker;
using ClassifiedAds.Modules.Storage.DTOs;
using ClassifiedAds.Modules.Storage.Entities;
using ClassifiedAds.Modules.Storage.Repositories;
using ClassifiedAds.Modules.Storage.Storages;
using ClassifiedAds.Modules.Storage.Storages.Amazon;
using ClassifiedAds.Modules.Storage.Storages.Azure;
using ClassifiedAds.Modules.Storage.Storages.Local;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class StorageServiceCollectionExtensions
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
            else
            {
                services.AddSingleton<IFileStorageManager>(new LocalFileStorageManager(storageOptions.Local.Path));
            }

            if (messageBrokerOptions.UsedRabbitMQ())
            {
                services.AddSingleton<IMessageSender<FileUploadedEvent>>(new RabbitMQSender<FileUploadedEvent>(new RabbitMQSenderOptions
                {
                    HostName = messageBrokerOptions.RabbitMQ.HostName,
                    UserName = messageBrokerOptions.RabbitMQ.UserName,
                    Password = messageBrokerOptions.RabbitMQ.Password,
                    ExchangeName = messageBrokerOptions.RabbitMQ.ExchangeName,
                    RoutingKey = messageBrokerOptions.RabbitMQ.RoutingKey_FileUploaded,
                }));

                services.AddSingleton<IMessageSender<FileDeletedEvent>>(new RabbitMQSender<FileDeletedEvent>(new RabbitMQSenderOptions
                {
                    HostName = messageBrokerOptions.RabbitMQ.HostName,
                    UserName = messageBrokerOptions.RabbitMQ.UserName,
                    Password = messageBrokerOptions.RabbitMQ.Password,
                    ExchangeName = messageBrokerOptions.RabbitMQ.ExchangeName,
                    RoutingKey = messageBrokerOptions.RabbitMQ.RoutingKey_FileDeleted,
                }));
            }
            else if (messageBrokerOptions.UsedKafka())
            {
                services.AddSingleton<IMessageSender<FileUploadedEvent>>(new KafkaSender<FileUploadedEvent>(
                    messageBrokerOptions.Kafka.BootstrapServers,
                    messageBrokerOptions.Kafka.Topic_FileUploaded));

                services.AddSingleton<IMessageSender<FileDeletedEvent>>(new KafkaSender<FileDeletedEvent>(
                    messageBrokerOptions.Kafka.BootstrapServers,
                    messageBrokerOptions.Kafka.Topic_FileDeleted));
            }
            else if (messageBrokerOptions.UsedAzureQueue())
            {
                services.AddSingleton<IMessageSender<FileUploadedEvent>>(new AzureQueueSender<FileUploadedEvent>(
                    connectionString: messageBrokerOptions.AzureQueue.ConnectionString,
                    queueName: messageBrokerOptions.AzureQueue.QueueName_FileUploaded));

                services.AddSingleton<IMessageSender<FileDeletedEvent>>(new AzureQueueSender<FileDeletedEvent>(
                    connectionString: messageBrokerOptions.AzureQueue.ConnectionString,
                    queueName: messageBrokerOptions.AzureQueue.QueueName_FileDeleted));
            }
            else if (messageBrokerOptions.UsedAzureServiceBus())
            {
                services.AddSingleton<IMessageSender<FileUploadedEvent>>(new AzureServiceBusSender<FileUploadedEvent>(
                    connectionString: messageBrokerOptions.AzureServiceBus.ConnectionString,
                    queueName: messageBrokerOptions.AzureServiceBus.QueueName_FileUploaded));

                services.AddSingleton<IMessageSender<FileDeletedEvent>>(new AzureServiceBusSender<FileDeletedEvent>(
                    connectionString: messageBrokerOptions.AzureServiceBus.ConnectionString,
                    queueName: messageBrokerOptions.AzureServiceBus.QueueName_FileDeleted));
            }
            else if (messageBrokerOptions.UsedAzureEventGrid())
            {
                services.AddSingleton<IMessageSender<FileUploadedEvent>>(new AzureEventGridSender<FileUploadedEvent>(
                                messageBrokerOptions.AzureEventGrid.DomainEndpoint,
                                messageBrokerOptions.AzureEventGrid.DomainKey,
                                messageBrokerOptions.AzureEventGrid.Topic_FileUploaded));

                services.AddSingleton<IMessageSender<FileDeletedEvent>>(new AzureEventGridSender<FileDeletedEvent>(
                                messageBrokerOptions.AzureEventGrid.DomainEndpoint,
                                messageBrokerOptions.AzureEventGrid.DomainKey,
                                messageBrokerOptions.AzureEventGrid.Topic_FileDeleted));
            }
            else if (messageBrokerOptions.UsedAzureEventHub())
            {
                services.AddSingleton<IMessageSender<FileUploadedEvent>>(new AzureEventHubSender<FileUploadedEvent>(
                                messageBrokerOptions.AzureEventHub.ConnectionString,
                                messageBrokerOptions.AzureEventHub.Hub_FileUploaded));

                services.AddSingleton<IMessageSender<FileDeletedEvent>>(new AzureEventHubSender<FileDeletedEvent>(
                                messageBrokerOptions.AzureEventHub.ConnectionString,
                                messageBrokerOptions.AzureEventHub.Hub_FileDeleted));
            }

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
