namespace ClassifiedAds.NotificationServer.ConfigurationOptions
{
    public class AppSettings
    {
        public string AllowedHosts { get; set; }

        public CORS CORS { get; set; }

        public Azure Azure { get; set; }
    }
}
