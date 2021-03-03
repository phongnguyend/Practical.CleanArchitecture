using CryptographyHelper.Certificates;

namespace ClassifiedAds.Infrastructure.Configuration
{
    public class ConfigurationProviders
    {
        public SqlServerOptions SqlServer { get; set; }

        public AzureAppConfigurationOptions AzureAppConfiguration { get; set; }

        public AzureKeyVaultOptions AzureKeyVault { get; set; }
    }

    public class SqlServerOptions
    {
        public bool IsEnabled { get; set; }

        public string ConnectionString { get; set; }

        public string SqlQuery { get; set; }

        public CertificateOption Certificate { get; set; }
    }

    public class AzureKeyVaultOptions
    {
        public bool IsEnabled { get; set; }

        public string VaultName { get; set; }
    }

    public class AzureAppConfigurationOptions
    {
        public bool IsEnabled { get; set; }

        public string ConnectionString { get; set; }
    }
}
