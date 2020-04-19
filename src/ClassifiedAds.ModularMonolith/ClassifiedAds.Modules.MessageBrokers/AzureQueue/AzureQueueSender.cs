using ClassifiedAds.Modules.MessageBrokers.Contracts.Services;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace ClassifiedAds.Modules.MessageBrokers.AzureQueue
{
    public class AzureQueueSender<T> : IMessageBusSender<T>
    {
        private readonly string _connectionString;
        private readonly string _queueName;

        public AzureQueueSender(string connectionString, string queueName)
        {
            _connectionString = connectionString;
            _queueName = queueName;
        }

        public void Send(T message)
        {
            SendAsync(message).Wait();
        }

        private async Task SendAsync(T message)
        {
            var storageAccount = CloudStorageAccount.Parse(_connectionString);
            var queueClient = storageAccount.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference(_queueName);
            await queue.CreateIfNotExistsAsync();
            var jsonMessage = new CloudQueueMessage(JsonConvert.SerializeObject(message));
            await queue.AddMessageAsync(jsonMessage);
        }
    }
}
