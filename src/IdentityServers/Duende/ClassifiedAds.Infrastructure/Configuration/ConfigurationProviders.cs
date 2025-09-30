namespace ClassifiedAds.Infrastructure.Configuration;

public class ConfigurationProviders
{
    public SqlServerOptions SqlServer { get; set; }

    public AzureAppConfigurationOptions AzureAppConfiguration { get; set; }

    public AzureKeyVaultOptions AzureKeyVault { get; set; }

    public HashiCorpVaultOptions HashiCorpVault { get; set; }
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
