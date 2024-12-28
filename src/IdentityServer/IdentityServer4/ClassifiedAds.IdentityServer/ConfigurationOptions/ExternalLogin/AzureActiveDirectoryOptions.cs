namespace ClassifiedAds.IdentityServer.ConfigurationOptions.ExternalLogin;

public class AzureActiveDirectoryOptions
{
    public bool IsEnabled { get; set; }

    public string Authority { get; set; }

    public string ClientId { get; set; }

    public string ClientSecret { get; set; }
}
