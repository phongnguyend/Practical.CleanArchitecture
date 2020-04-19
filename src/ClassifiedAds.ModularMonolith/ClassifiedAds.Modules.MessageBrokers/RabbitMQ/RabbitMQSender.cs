using ClassifiedAds.Modules.MessageBrokers.Contracts.Services;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace ClassifiedAds.Modules.MessageBrokers.RabbitMQ
{
    public class RabbitMQSender<T> : IMessageBusSender<T>
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly string _exchangeName;
        private readonly string _routingKey;

        public RabbitMQSender(RabbitMQSenderOptions options)
        {
            _connectionFactory = new ConnectionFactory
            {
                HostName = options.HostName,
                UserName = options.UserName,
                Password = options.Password,
            };

            _exchangeName = options.ExchangeName;
            _routingKey = options.RoutingKey;
        }

        public void Send(T message)
        {
            using (var connection = _connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish(exchange: _exchangeName,
                                     routingKey: _routingKey,
                                     basicProperties: properties,
                                     body: body);
            }
        }
    }
}
