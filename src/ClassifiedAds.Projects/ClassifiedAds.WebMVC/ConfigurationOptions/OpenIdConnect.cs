namespace ClassifiedAds.WebMVC.ConfigurationOptions
{
    public class OpenIdConnect
    {
        public string Authority { get; set; }

        public string ClientId { get; set; }

        public bool RequireHttpsMetadata { get; set; }
    }
}
