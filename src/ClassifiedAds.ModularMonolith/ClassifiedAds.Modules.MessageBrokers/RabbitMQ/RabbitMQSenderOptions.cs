namespace ClassifiedAds.Modules.MessageBrokers.RabbitMQ
{
    public class RabbitMQSenderOptions
    {
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ExchangeName { get; set; }
        public string RoutingKey { get; set; }
    }
}
