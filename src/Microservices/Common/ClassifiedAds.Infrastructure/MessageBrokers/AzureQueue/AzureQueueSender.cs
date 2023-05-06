using Azure.Storage.Queues;
using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.MessageBrokers.AzureQueue;

public class AzureQueueSender<T> : IMessageSender<T>
{
    private readonly string _connectionString;
    private readonly string _queueName;

    public AzureQueueSender(string connectionString, string queueName)
    {
        _connectionString = connectionString;
        _queueName = queueName;
    }

    public async Task SendAsync(T message, MetaData metaData, CancellationToken cancellationToken = default)
    {
        var queueClient = new QueueClient(_connectionString, _queueName);
        await queueClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        var jsonMessage = JsonSerializer.Serialize(new Message<T>
        {
            Data = message,
            MetaData = metaData,
        });

        await queueClient.SendMessageAsync(jsonMessage, cancellationToken);
    }
}
