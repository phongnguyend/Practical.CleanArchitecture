namespace ClassifiedAds.WebMVC.ConfigurationOptions.MessageBroker
{
    public class MessageBrokerOptions
    {
        public string Provider { get; set; }

        public RabbitMQOptions RabbitMQ { get; set; }

        public AzureQueueOptions AzureQueue { get; set; }

        public AzureServiceBusOptions AzureServiceBus { get; set; }

        public bool UsedRabbitMQ()
        {
            return Provider == "RabbitMQ";
        }

        public bool UsedAzureQueue()
        {
            return Provider == "AzureQueue";
        }

        public bool UsedAzureServiceBus()
        {
            return Provider == "AzureServiceBus";
        }
    }
}
