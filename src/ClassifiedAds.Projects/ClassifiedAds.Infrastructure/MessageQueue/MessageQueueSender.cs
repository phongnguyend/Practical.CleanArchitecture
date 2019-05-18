using ClassifiedAds.DomainServices.Infrastructure;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace ClassifiedAds.Infrastructure
{
    public class MessageQueueSender<T> : IMessageQueueSender<T>
    {
        private readonly IConnectionFactory _connectionFactory;

        public MessageQueueSender(string hostName, string userName, string password)
        {
            _connectionFactory = new ConnectionFactory
            {
                HostName = hostName,
                UserName = userName,
                Password = password
            };
        }

        public void Send(T message, string exchangeName, string routingKey)
        {
            using (var connection = _connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish(exchange: exchangeName,
                                     routingKey: routingKey,
                                     basicProperties: properties,
                                     body: body);
            }
        }
    }
}
