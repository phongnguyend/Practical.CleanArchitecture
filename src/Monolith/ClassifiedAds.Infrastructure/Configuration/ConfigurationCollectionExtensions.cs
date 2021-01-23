using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using System;

namespace ClassifiedAds.Infrastructure.Configuration
{
    public static class ConfigurationCollectionExtensions
    {
        public static IConfigurationBuilder AddAppConfiguration(this IConfigurationBuilder configurationBuilder, ConfigurationProviders options)
        {
            if (options?.SqlServer?.IsEnabled ?? false)
            {
                configurationBuilder.AddSqlConfigurationVariables(options.SqlServer);
            }

            if (options?.AzureAppConfiguration?.IsEnabled ?? false)
            {
                configurationBuilder.AddAzureAppConfiguration(options.AzureAppConfiguration.ConnectionString);
            }

            if (options?.AzureKeyVault?.IsEnabled ?? false)
            {
                var secretClient = new SecretClient(new Uri(options.AzureKeyVault.VaultName), new DefaultAzureCredential());
                configurationBuilder.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
            }

            return configurationBuilder;
        }
    }
}
