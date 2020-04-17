namespace ClassifiedAds.BackgroundServer.ConfigurationOptions.MessageBroker
{
    public class RabbitMQOptions
    {
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ExchangeName { get; set; }
        public string RoutingKey_FileUploaded { get; set; }
        public string RoutingKey_FileDeleted { get; set; }
        public string QueueName_FileUploaded { get; set; }
        public string QueueName_FileDeleted { get; set; }

        public string ConnectionString
        {
            get
            {
                return $"amqp://{UserName}:{Password}@{HostName}/%2f";
            }
        }
    }
}
