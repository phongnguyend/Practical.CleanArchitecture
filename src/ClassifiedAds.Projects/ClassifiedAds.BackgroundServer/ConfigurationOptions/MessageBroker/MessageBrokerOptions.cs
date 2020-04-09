namespace ClassifiedAds.BackgroundServer.ConfigurationOptions.MessageBroker
{
    public class MessageBrokerOptions
    {
        public string Provider { get; set; }

        public RabbitMQOptions RabbitMQ { get; set; }

        public KafkaOptions Kafka { get; set; }

        public AzureQueueOptions AzureQueue { get; set; }

        public AzureServiceBusOptions AzureServiceBus { get; set; }

        public bool UsedRabbitMQ()
        {
            return Provider == "RabbitMQ";
        }

        public bool UsedKafka()
        {
            return Provider == "Kafka";
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
