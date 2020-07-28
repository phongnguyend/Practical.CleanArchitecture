namespace ClassifiedAds.BackgroundServer.ConfigurationOptions.MessageBroker
{
    public class AzureEventHubOptions
    {
        public string ConnectionString { get; set; }

        public string Hub_FileUploaded { get; set; }

        public string Hub_FileDeleted { get; set; }

        public string StorageConnectionString { get; set; }

        public string StorageContainerName_FileUploaded { get; set; }

        public string StorageContainerName_FileDeleted { get; set; }
    }
}
