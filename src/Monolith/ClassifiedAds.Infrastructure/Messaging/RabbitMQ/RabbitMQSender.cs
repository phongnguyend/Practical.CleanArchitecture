using ClassifiedAds.Domain.Infrastructure.Messaging;
using CryptographyHelper;
using CryptographyHelper.SymmetricAlgorithms;
using RabbitMQ.Client;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Messaging.RabbitMQ;

public class RabbitMQSender<T> : IMessageSender<T>
{
    private readonly RabbitMQSenderOptions _options;
    private readonly ConnectionFactory _connectionFactory;
    private readonly string _exchangeName;
    private readonly string _routingKey;

    public RabbitMQSender(RabbitMQSenderOptions options)
    {
        _options = options;

        _connectionFactory = new ConnectionFactory
        {
            HostName = options.HostName,
            UserName = options.UserName,
            Password = options.Password,
        };

        _exchangeName = options.ExchangeName;
        _routingKey = options.RoutingKey;
    }

    public async Task SendAsync(T message, MetaData metaData = null, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);
        var body = new Message<T>
        {
            Data = message,
            MetaData = metaData,
        }.GetBytes();

        var properties = new BasicProperties
        {
            Persistent = true
        };

        if (_options.MessageEncryptionEnabled)
        {
            var iv = SymmetricCrypto.GenerateKey(16);

            body = body.UseAES(_options.MessageEncryptionKey.FromBase64String())
            .WithCipher(CipherMode.CBC)
            .WithIV(iv)
            .WithPadding(PaddingMode.PKCS7)
            .Encrypt();

            properties.Headers = new Dictionary<string, object>
            {
                { "x-encrypted", true },
                { "x-encrypted-iv", iv.ToBase64String() }
            };
        }

        await channel.BasicPublishAsync(exchange: _exchangeName,
                               routingKey: _routingKey,
                               mandatory: true,
                               basicProperties: properties,
                               body: body,
                               cancellationToken: cancellationToken);
    }
}
