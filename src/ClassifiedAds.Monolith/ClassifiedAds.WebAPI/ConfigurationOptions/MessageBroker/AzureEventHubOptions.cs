namespace ClassifiedAds.WebAPI.ConfigurationOptions.MessageBroker
{
    public class AzureEventHubOptions
    {
        public string ConnectionString { get; set; }

        public string Hub_FileUploaded { get; set; }

        public string Hub_FileDeleted { get; set; }
    }
}
