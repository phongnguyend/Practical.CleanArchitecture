namespace ClassifiedAds.WebMVC.ConfigurationOptions.MessageBroker
{
    public class AzureQueueOptions
    {
        public string ConnectionString { get; set; }

        public string QueueName_FileUploaded { get; set; }

        public string QueueName_FileDeleted { get; set; }
    }
}
