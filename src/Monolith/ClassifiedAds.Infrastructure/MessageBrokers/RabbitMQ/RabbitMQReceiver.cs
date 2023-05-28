using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.MessageBrokers.RabbitMQ;

public class RabbitMQReceiver<T> : IMessageReceiver<T>, IDisposable
{
    private readonly RabbitMQReceiverOptions _options;
    private readonly IConnection _connection;
    private IModel _channel;
    private string _queueName;

    public RabbitMQReceiver(RabbitMQReceiverOptions options)
    {
        _options = options;

        _connection = new ConnectionFactory
        {
            HostName = options.HostName,
            UserName = options.UserName,
            Password = options.Password,
            AutomaticRecoveryEnabled = true,
            DispatchConsumersAsync = true
        }.CreateConnection();

        _queueName = options.QueueName;

        _connection.ConnectionShutdown += Connection_ConnectionShutdown;
    }

    private void Connection_ConnectionShutdown(object sender, ShutdownEventArgs e)
    {
        // TODO: add log here
    }

    public Task ReceiveAsync(Func<T, MetaData, Task> action, CancellationToken cancellationToken)
    {
        _channel = _connection.CreateModel();

        if (_options.AutomaticCreateEnabled)
        {
            _channel.QueueDeclare(_options.QueueName, true, false, false, null);
            _channel.QueueBind(_options.QueueName, _options.ExchangeName, _options.RoutingKey, null);
        }

        _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = Encoding.UTF8.GetString(ea.Body.Span);
            var message = JsonSerializer.Deserialize<Message<T>>(body);
            await action(message.Data, message.MetaData);
            _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        };
        _channel.BasicConsume(queue: _queueName,
                             autoAck: false,
                             consumer: consumer);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel.Dispose();
        _connection.Dispose();
    }
}
