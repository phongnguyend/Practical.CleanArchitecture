using ClassifiedAds.DomainServices.Infrastructure;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace ClassifiedAds.Infrastructure
{
    public class MessageQueueReceiver<T> : IMessageQueueReceiver<T>
    {
        private IConnection _connection;
        private IModel _channel;
        private string _queueName;

        public MessageQueueReceiver(string hostName, string userName, string password, string queueName)
        {
            _connection = new ConnectionFactory
            {
                HostName = hostName,
                UserName = userName,
                Password = password,
                AutomaticRecoveryEnabled = true
            }.CreateConnection();

            _queueName = queueName;

            _connection.ConnectionShutdown += connection_ConnectionShutdown;
        }

        private void connection_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            //TODO: add log here
        }

        public void Receive(Action<T> action)
        {
            _channel = _connection.CreateModel();

            /*In order to defeat that we can use the basicQos method with the prefetchCount = 1 setting.
             This tells RabbitMQ not to give more than one message to a worker at a time. 
             Or, in other words, don't dispatch a new message to a worker until it has processed and acknowledged the previous one. 
             Instead, it will dispatch it to the next worker that is not still busy.*/
            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = Encoding.UTF8.GetString(ea.Body);
                var message = JsonConvert.DeserializeObject<T>(body);
                action(message);
                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };
            _channel.BasicConsume(queue: _queueName,
                                 autoAck: false,
                                 consumer: consumer);
        }
    }
}
