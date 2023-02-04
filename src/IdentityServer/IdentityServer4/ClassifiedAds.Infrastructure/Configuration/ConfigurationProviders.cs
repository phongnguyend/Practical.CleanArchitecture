using CryptographyHelper.Certificates;

namespace ClassifiedAds.Infrastructure.Configuration
{
    public class ConfigurationProviders
    {
        public SqlServerOptions SqlServer { get; set; }

        public AzureAppConfigurationOptions AzureAppConfiguration { get; set; }

        public AzureKeyVaultOptions AzureKeyVault { get; set; }

        public HashiCorpVaultOptions HashiCorpVault { get; set; }
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

    public class HashiCorpVaultOptions
    {
        public bool IsEnabled { get; set; }

        public string Address { get; set; }

        public string SecretEnginePath { get; set; }

        public string SecretPath { get; set; }

        public string AuthMethod { get; set; }

        public HashiCorpVaultAuthOptions Auth { get; set; }
    }

    public class HashiCorpVaultAuthOptions
    {
        public HashiCorpVaultTokenAuthOptions Token { get; set; }

        public HashiCorpVaultUserPassAuthOptions UserPass { get; set; }
    }

    public class HashiCorpVaultUserPassAuthOptions
    {
        public string UserName { get; set; }

        public string Password { get; set; }
    }

    public class HashiCorpVaultTokenAuthOptions
    {
        public string Token { get; set; }
    }
}
