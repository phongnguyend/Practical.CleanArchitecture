namespace ClassifiedAds.Infrastructure.MessageBrokers.RabbitMQ
{
    public class RabbitMQReceiverOptions
    {
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string QueueName { get; set; }
        public bool AutomaticCreateEnabled { get; set; }
        public string ExchangeName { get; set; }
        public string RoutingKey { get; set; }
    }
}
