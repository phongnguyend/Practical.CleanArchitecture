namespace ClassifiedAds.Modules.Storage
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

    public class AmazonOptions
    {
        public string AccessKeyID { get; set; }

        public string SecretAccessKey { get; set; }

        public string BucketName { get; set; }

        public string RegionEndpoint { get; set; }
    }

    public class AzureBlobOption
    {
        public string ConnectionString { get; set; }

        public string Container { get; set; }
    }

    public class LocalOption
    {
        public string Path { get; set; }
    }
}
