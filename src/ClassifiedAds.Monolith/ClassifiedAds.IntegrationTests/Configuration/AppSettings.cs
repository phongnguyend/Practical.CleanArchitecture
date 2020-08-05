namespace ClassifiedAds.IntegrationTests.Configuration
{
    public class AppSettings
    {
        public OpenIdConnect OpenIdConnect { get; set; }

        public ResourceServer WebAPI { get; set; }

        public ResourceServer Ocelot { get; set; }

        public ResourceServer GraphQL { get; set; }

        public LoginOptions Login { get; set; }
    }
}
