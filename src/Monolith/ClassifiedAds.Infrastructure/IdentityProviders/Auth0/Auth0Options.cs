namespace ClassifiedAds.Infrastructure.IdentityProviders.Auth0;

public class Auth0Options
{
    public bool Enabled { get; set; }

    public string TokenUrl { get; set; }

    public string ClientId { get; set; }

    public string ClientSecret { get; set; }

    public string Audience { get; set; }
}
