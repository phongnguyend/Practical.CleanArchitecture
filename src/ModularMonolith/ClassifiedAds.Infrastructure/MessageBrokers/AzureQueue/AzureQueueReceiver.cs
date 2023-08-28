using Azure.Storage.Queues;
using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.MessageBrokers.AzureQueue;

public class AzureQueueReceiver<TConsumer, T> : IMessageReceiver<TConsumer, T>
{
    private readonly string _connectionString;
    private readonly string _queueName;
    private readonly QueueMessageEncoding _messageEncoding;

    public AzureQueueReceiver(string connectionString, string queueName, QueueMessageEncoding messageEncoding = QueueMessageEncoding.None)
    {
        _connectionString = connectionString;
        _queueName = queueName;
        _messageEncoding = messageEncoding;
    }

    public async Task ReceiveAsync(Func<T, MetaData, Task> action, CancellationToken cancellationToken)
    {
        await ReceiveStringAsync(async retrievedMessage =>
        {
            var message = JsonSerializer.Deserialize<Message<T>>(retrievedMessage);
            await action(message.Data, message.MetaData);
        }, cancellationToken);
    }

    private async Task ReceiveStringAsync(Func<string, Task> action, CancellationToken cancellationToken)
    {
        var queueClient = new QueueClient(_connectionString, _queueName, new QueueClientOptions
        {
            MessageEncoding = _messageEncoding,
        });

        await queueClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var retrievedMessages = (await queueClient.ReceiveMessagesAsync(cancellationToken)).Value;

                if (retrievedMessages.Length > 0)
                {
                    foreach (var retrievedMessage in retrievedMessages)
                    {
                        await action(retrievedMessage.Body.ToString());
                        await queueClient.DeleteMessageAsync(retrievedMessage.MessageId, retrievedMessage.PopReceipt, cancellationToken);
                    }
                }
                else
                {
                    await Task.Delay(1000, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await Task.Delay(1000, cancellationToken);
            }
        }
    }
}
