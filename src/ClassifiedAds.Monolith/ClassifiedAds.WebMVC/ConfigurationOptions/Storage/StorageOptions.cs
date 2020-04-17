namespace ClassifiedAds.WebMVC.ConfigurationOptions.Storage
{
    public class StorageOptions
    {
        public string Provider { get; set; }

        public LocalOption Local { get; set; }

        public AzureBlobOption Azure { get; set; }

        public AmazonOptions Amazon { get; set; }

        public bool UsedLocal()
        {
            return Provider == "Local";
        }

        public bool UsedAzure()
        {
            return Provider == "Azure";
        }

        public bool UsedAmazon()
        {
            return Provider == "Amazon";
        }
    }
}
