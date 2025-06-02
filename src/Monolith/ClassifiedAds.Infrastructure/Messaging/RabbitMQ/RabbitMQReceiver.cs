using ClassifiedAds.Domain.Infrastructure.Messaging;
using CryptographyHelper;
using CryptographyHelper.SymmetricAlgorithms;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Messaging.RabbitMQ;

public class RabbitMQReceiver<TConsumer, T> : IMessageReceiver<TConsumer, T>, IDisposable
{
    private readonly RabbitMQReceiverOptions _options;
    private IConnection _connection;
    private IChannel _channel;

    public RabbitMQReceiver(RabbitMQReceiverOptions options)
    {
        _options = options;
    }

    private Task Connection_ConnectionShutdownAsync(object sender, ShutdownEventArgs e)
    {
        // TODO: add log here

        return Task.CompletedTask;
    }

    public async Task ReceiveAsync(Func<T, MetaData, Task> action, CancellationToken cancellationToken = default)
    {
        _connection = await new ConnectionFactory
        {
            HostName = _options.HostName,
            UserName = _options.UserName,
            Password = _options.Password,
            AutomaticRecoveryEnabled = true,
        }.CreateConnectionAsync(cancellationToken);

        _connection.ConnectionShutdownAsync += Connection_ConnectionShutdownAsync;

        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        if (_options.AutomaticCreateEnabled)
        {
            var arguments = new Dictionary<string, object>();

            if (string.Equals(_options.QueueType, "Quorum", StringComparison.OrdinalIgnoreCase))
            {
                arguments["x-queue-type"] = "quorum";
            }
            else if (string.Equals(_options.QueueType, "Stream", StringComparison.OrdinalIgnoreCase))
            {
                arguments["x-queue-type"] = "stream";
            }

            if (_options.SingleActiveConsumer)
            {
                arguments["x-single-active-consumer"] = true;
            }

            if (_options.DeadLetter != null)
            {
                if (!string.IsNullOrEmpty(_options.DeadLetter.ExchangeName))
                {
                    arguments["x-dead-letter-exchange"] = _options.DeadLetter.ExchangeName;
                }

                if (!string.IsNullOrEmpty(_options.DeadLetter.RoutingKey))
                {
                    arguments["x-dead-letter-routing-key"] = _options.DeadLetter.RoutingKey;
                }

                if (_options.DeadLetter.AutomaticCreateEnabled && !string.IsNullOrEmpty(_options.DeadLetter.QueueName))
                {
                    await _channel.QueueDeclareAsync(_options.DeadLetter.QueueName, true, false, false, null, cancellationToken: cancellationToken);
                    await _channel.QueueBindAsync(_options.DeadLetter.QueueName, _options.DeadLetter.ExchangeName, _options.DeadLetter.RoutingKey, null, cancellationToken: cancellationToken);
                }
            }

            arguments = arguments.Count == 0 ? null : arguments;

            await _channel.QueueDeclareAsync(_options.QueueName, true, false, false, arguments, cancellationToken: cancellationToken);
            await _channel.QueueBindAsync(_options.QueueName, _options.ExchangeName, _options.RoutingKey, null, cancellationToken: cancellationToken);
            await _channel.QueueBindAsync(_options.QueueName, "amq.direct", $"direct_route_to_queue_{_options.QueueName}", null, cancellationToken: cancellationToken);
        }

        await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false, cancellationToken: cancellationToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var bodyText = string.Empty;

                if (_options.MessageEncryptionEnabled)
                {
                    var parts = Encoding.UTF8.GetString(ea.Body.Span).Split('.');

                    var iv = parts[0].FromBase64String();
                    var encryptedBytes = parts[1].FromBase64String();

                    bodyText = encryptedBytes.UseAES(_options.MessageEncryptionKey.FromBase64String())
                    .WithCipher(CipherMode.CBC)
                    .WithIV(iv)
                    .WithPadding(PaddingMode.PKCS7)
                    .Decrypt()
                    .GetString();
                }
                else
                {
                    bodyText = Encoding.UTF8.GetString(ea.Body.Span);
                }

                var message = JsonSerializer.Deserialize<Message<T>>(bodyText);

                await action(message.Data, message.MetaData);

                await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                // TODO: log here
                await Task.Delay(1000);
                await _channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: _options.RequeueOnFailure);
            }
        };

        await _channel.BasicConsumeAsync(queue: _options.QueueName, autoAck: false, consumer: consumer, cancellationToken: cancellationToken);
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
