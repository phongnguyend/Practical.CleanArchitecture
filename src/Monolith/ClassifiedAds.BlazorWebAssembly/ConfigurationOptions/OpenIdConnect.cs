namespace ClassifiedAds.BlazorWebAssembly.ConfigurationOptions;

public class OpenIdConnect
{
    public string Authority { get; set; }

    public string ClientId { get; set; }

    public string RedirectUri { get; set; }

    public string PostLogoutRedirectUri { get; set; }
}
