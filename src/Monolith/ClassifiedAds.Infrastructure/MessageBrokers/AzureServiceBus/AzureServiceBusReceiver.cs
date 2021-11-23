using Azure.Messaging.ServiceBus;
using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.MessageBrokers.AzureServiceBus
{
    public class AzureServiceBusReceiver<T> : IMessageReceiver<T>
    {
        private readonly string _connectionString;
        private readonly string _queueName;

        public AzureServiceBusReceiver(string connectionString, string queueName)
        {
            _connectionString = connectionString;
            _queueName = queueName;
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
            await using var client = new ServiceBusClient(_connectionString);
            ServiceBusReceiver receiver = client.CreateReceiver(_queueName);

            while (true)
            {
                var retrievedMessage = await receiver.ReceiveMessageAsync();

                if (retrievedMessage != null)
                {
                    action(Encoding.UTF8.GetString(retrievedMessage.Body));
                    await receiver.CompleteMessageAsync(retrievedMessage);
                }
                else
                {
                    await Task.Delay(1000);
                }
            }
        }
    }
}
