using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.MessageBrokers.AzureServiceBus
{
    public class AzureServiceBusTopicSender<T> : IMessageSender<T>
    {
        private readonly string _connectionString;
        private readonly string _topicName;

        public AzureServiceBusTopicSender(string connectionString, string topicName)
        {
            _connectionString = connectionString;
            _topicName = topicName;
        }

        public void Send(T message, MetaData metaData = null)
        {
            SendAsync(message, metaData).Wait();
        }

        public async Task SendAsync(T message, MetaData metaData, CancellationToken cancellationToken = default)
        {
            var topicClient = new TopicClient(_connectionString, _topicName);
            var bytes = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new Message<T>
            {
                Data = message,
                MetaData = metaData,
            })));
            await topicClient.SendAsync(bytes);
        }
    }
}
