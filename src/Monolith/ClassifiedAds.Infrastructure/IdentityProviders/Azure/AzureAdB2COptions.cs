namespace ClassifiedAds.Infrastructure.IdentityProviders.Azure;

public class AzureAdB2COptions
{
    public bool Enabled { get; set; }

    public string TenantId { get; set; }

    public string TenantDomain { get; set; }

    public string AppId { get; set; }

    public string ClientSecret { get; set; }
}
