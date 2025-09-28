namespace ClassifiedAds.ApiIntegrationTests.Configuration;

public class OpenIdConnect
{
    public string Authority { get; set; }

    public string ClientId { get; set; }

    public string ClientSecret { get; set; }

    public bool RequireHttpsMetadata { get; set; }
}
