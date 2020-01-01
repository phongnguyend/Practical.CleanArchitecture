namespace ClassifiedAds.WebMVC.ConfigurationOptions.MessageBroker
{
    public class AzureServiceBusOptions
    {
        public string ConnectionString { get; set; }

        public string QueueName_FileUploaded { get; set; }

        public string QueueName_FileDeleted { get; set; }
    }
}
