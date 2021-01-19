using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.MessageBrokers.AzureQueue
{
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
            var storageAccount = CloudStorageAccount.Parse(_connectionString);
            var queueClient = storageAccount.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference(_queueName);
            await queue.CreateIfNotExistsAsync();
            var jsonMessage = new CloudQueueMessage(JsonConvert.SerializeObject(new Message<T>
            {
                Data = message,
                MetaData = metaData,
            }));
            await queue.AddMessageAsync(jsonMessage, cancellationToken);
        }
    }
}
