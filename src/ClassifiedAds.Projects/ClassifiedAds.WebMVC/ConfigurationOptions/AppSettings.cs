namespace ClassifiedAds.WebMVC.ConfigurationOptions
{
    public class AppSettings
    {
        public ConnectionStrings ConnectionStrings { get; set; }

        public OpenIdConnect OpenIdConnect { get; set; }

        public ResourceServer ResourceServer { get; set; }

        public string AllowedHosts { get; set; }
    }
}
