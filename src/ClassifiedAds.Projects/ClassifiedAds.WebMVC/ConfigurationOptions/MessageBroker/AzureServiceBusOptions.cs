namespace ClassifiedAds.WebMVC.ConfigurationOptions.MessageBroker
{
    public class AzureServiceBusOptions
    {
        public string ConnectionString { get; set; }

        public string QueueName { get; set; }
    }
}
