namespace ClassifiedAds.Modules.Identity.ConfigurationOptions;

public class IdentityServerOptions
{
    public string Authority { get; set; }

    public string ApiName { get; set; }

    public bool RequireHttpsMetadata { get; set; }
}
