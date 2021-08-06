using Amazon;
using Amazon.S3;
using ClassifiedAds.Domain.Infrastructure.Storages;
using ClassifiedAds.Infrastructure.Storages;
using ClassifiedAds.Infrastructure.Storages.Amazon;
using ClassifiedAds.Infrastructure.Storages.Azure;
using ClassifiedAds.Infrastructure.Storages.Fake;
using ClassifiedAds.Infrastructure.Storages.Local;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class StoragesCollectionExtensions
    {
        public static IServiceCollection AddLocalStorageManager(this IServiceCollection services, LocalOption options)
        {
            services.AddSingleton<IFileStorageManager>(new LocalFileStorageManager(options));

            return services;
        }

        public static IServiceCollection AddAzureBlobStorageManager(this IServiceCollection services, AzureBlobOption options)
        {
            services.AddSingleton<IFileStorageManager>(new AzureBlobStorageManager(options));

            return services;
        }

        public static IServiceCollection AddAmazonS3StorageManager(this IServiceCollection services, AmazonOptions options)
        {
            services.AddSingleton<IFileStorageManager>(new AmazonS3StorageManager(options));

            return services;
        }

        public static IServiceCollection AddFakeStorageManager(this IServiceCollection services)
        {
            services.AddSingleton<IFileStorageManager>(new FakeStorageManager());

            return services;
        }

        public static IServiceCollection AddStorageManager(this IServiceCollection services, StorageOptions options, IHealthChecksBuilder healthChecksBuilder = null)
        {
            if (options.UsedAzure())
            {
                services.AddAzureBlobStorageManager(options.Azure);

                if (healthChecksBuilder != null)
                {
                    healthChecksBuilder.AddAzureBlobStorage(
                        options.Azure.ConnectionString,
                        containerName: options.Azure.Container,
                        name: "Storage (Azure Blob)",
                        failureStatus: HealthStatus.Degraded);
                }
            }
            else if (options.UsedAmazon())
            {
                services.AddAmazonS3StorageManager(options.Amazon);

                if (healthChecksBuilder != null)
                {
                    healthChecksBuilder.AddS3(
                    s3 =>
                    {
                        s3.AccessKey = options.Amazon.AccessKeyID;
                        s3.SecretKey = options.Amazon.SecretAccessKey;
                        s3.BucketName = options.Amazon.BucketName;
                        s3.S3Config = new AmazonS3Config
                        {
                            RegionEndpoint = RegionEndpoint.GetBySystemName(options.Amazon.RegionEndpoint),
                        };
                    },
                    name: "Storage (Amazon S3)",
                    failureStatus: HealthStatus.Degraded);
                }
            }
            else if (options.UsedLocal())
            {
                services.AddLocalStorageManager(options.Local);

                if (healthChecksBuilder != null)
                {
                    healthChecksBuilder.AddFilePathWrite(options.Local.Path,
                    name: "Storage (Local Directory)",
                    failureStatus: HealthStatus.Degraded);
                }
            }
            else
            {
                services.AddFakeStorageManager();
            }

            return services;
        }
    }
}
