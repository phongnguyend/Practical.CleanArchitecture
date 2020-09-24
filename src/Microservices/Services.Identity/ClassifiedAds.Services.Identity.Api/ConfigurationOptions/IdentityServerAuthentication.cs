namespace ClassifiedAds.Services.Identity.ConfigurationOptions
{
    public class IdentityServerAuthentication
    {
        public string Authority { get; set; }

        public string ApiName { get; set; }

        public bool RequireHttpsMetadata { get; set; }
    }
}
