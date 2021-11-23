using Azure.Storage.Queues;
using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.MessageBrokers.AzureQueue
{
    public class AzureQueueReceiver<T> : IMessageReceiver<T>
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

        public void Receive(Action<T, MetaData> action)
        {
            Task.Factory.StartNew(() => ReceiveAsync(action));
        }

        private Task ReceiveAsync(Action<T, MetaData> action)
        {
            return ReceiveStringAsync(retrievedMessage =>
            {
                var message = JsonSerializer.Deserialize<Message<T>>(retrievedMessage);
                action(message.Data, message.MetaData);
            });
        }

        public void ReceiveString(Action<string> action)
        {
            Task.Factory.StartNew(() => ReceiveStringAsync(action));
        }

        private async Task ReceiveStringAsync(Action<string> action)
        {
            var queueClient = new QueueClient(_connectionString, _queueName, new QueueClientOptions
            {
                MessageEncoding = _messageEncoding,
            });

            await queueClient.CreateIfNotExistsAsync();

            while (true)
            {
                try
                {
                    var retrievedMessages = (await queueClient.ReceiveMessagesAsync()).Value;

                    if (retrievedMessages.Length > 0)
                    {
                        foreach (var retrievedMessage in retrievedMessages)
                        {
                            action(retrievedMessage.Body.ToString());
                            await queueClient.DeleteMessageAsync(retrievedMessage.MessageId, retrievedMessage.PopReceipt);
                        }
                    }
                    else
                    {
                        await Task.Delay(1000);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    await Task.Delay(1000);
                }
            }
        }
    }
}
