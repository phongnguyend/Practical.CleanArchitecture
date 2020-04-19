using ClassifiedAds.Modules.MessageBrokers.Contracts.Services;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace ClassifiedAds.Modules.MessageBrokers.RabbitMQ
{
    public class RabbitMQReceiver<T> : IMessageBusReceiver<T>, IDisposable
    {
        private IConnection _connection;
        private IModel _channel;
        private string _queueName;
        private readonly RabbitMQReceiverOptions _options;

        public RabbitMQReceiver(RabbitMQReceiverOptions options)
        {
            _options = options;

            _connection = new ConnectionFactory
            {
                HostName = options.HostName,
                UserName = options.UserName,
                Password = options.Password,
                AutomaticRecoveryEnabled = true,
            }.CreateConnection();

            _queueName = options.QueueName;

            _connection.ConnectionShutdown += Connection_ConnectionShutdown;
        }

        private void Connection_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            // TODO: add log here
        }

        public void Receive(Action<T> action)
        {
            _channel = _connection.CreateModel();

            if (_options.AutomaticCreateEnabled)
            {
                _channel.QueueDeclare(_options.QueueName, true, false, false, null);
                _channel.QueueBind(_options.QueueName, _options.ExchangeName, _options.RoutingKey, null);
            }

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

        public void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();
        }
    }
}
