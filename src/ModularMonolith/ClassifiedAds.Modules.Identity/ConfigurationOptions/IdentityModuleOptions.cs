namespace ClassifiedAds.Modules.Identity.ConfigurationOptions;

public class IdentityModuleOptions
{
    public ConnectionStringsOptions ConnectionStrings { get; set; }

    public IdentityServerOptions IdentityServer { get; set; }

    public IdentityProvidersOptions Providers { get; set; }
}
