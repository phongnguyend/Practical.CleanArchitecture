using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.MessageBrokers.AzureQueue
{
    public class AzureQueueReceiver<T> : IMessageReceiver<T>
    {
        private readonly string _connectionString;
        private readonly string _queueName;

        public AzureQueueReceiver(string connectionString, string queueName)
        {
            _connectionString = connectionString;
            _queueName = queueName;
        }

        public void Receive(Action<T> action)
        {
            Task.Factory.StartNew(() => ReceiveAsync(action));
        }

        private async Task ReceiveAsync(Action<T> action)
        {
            var storageAccount = CloudStorageAccount.Parse(_connectionString);
            var queueClient = storageAccount.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference(_queueName);

            await queue.CreateIfNotExistsAsync();

            while (true)
            {
                var retrievedMessage = await queue.GetMessageAsync();

                if (retrievedMessage != null)
                {
                    var message = JsonConvert.DeserializeObject<T>(retrievedMessage.AsString);
                    action(message);
                    await queue.DeleteMessageAsync(retrievedMessage);
                }
                else
                {
                    await Task.Delay(1000);
                }
            }
        }
    }
}
