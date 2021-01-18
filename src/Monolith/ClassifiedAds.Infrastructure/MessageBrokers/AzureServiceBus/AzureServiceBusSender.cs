using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.MessageBrokers.AzureServiceBus
{
    public class AzureServiceBusSender<T> : IMessageSender<T>
    {
        private readonly string _connectionString;
        private readonly string _queueName;

        public AzureServiceBusSender(string connectionString, string queueName)
        {
            _connectionString = connectionString;
            _queueName = queueName;
        }

        public async Task SendAsync(T message, MetaData metaData, CancellationToken cancellationToken = default)
        {
            var queueClient = new QueueClient(_connectionString, _queueName);
            var bytes = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new Message<T>
            {
                Data = message,
                MetaData = metaData,
            })));
            await queueClient.SendAsync(bytes);
        }
    }
}
