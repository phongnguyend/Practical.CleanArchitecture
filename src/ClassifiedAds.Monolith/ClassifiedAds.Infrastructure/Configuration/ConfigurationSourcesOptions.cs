namespace ClassifiedAds.Infrastructure.Configuration
{
    public class ConfigurationSourcesOptions
    {
        public SqlServerSourceOptions SqlServer { get; set; }

        public AzureKeyVaultOptions AzureKeyVault { get; set; }
    }

    public class SqlServerSourceOptions
    {
        public bool IsEnabled { get; set; }

        public string ConnectionString { get; set; }

        public string SqlQuery { get; set; }
    }

    public class AzureKeyVaultOptions
    {
        public bool IsEnabled { get; set; }

        public string VaultName { get; set; }
    }
}
