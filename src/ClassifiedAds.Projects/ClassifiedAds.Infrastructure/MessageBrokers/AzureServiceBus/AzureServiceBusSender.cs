using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System.Text;
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

        public void Send(T message)
        {
            SendAsync(message).Wait();
        }

        private async Task SendAsync(T message)
        {
            var queueClient = new QueueClient(_connectionString, _queueName);
            var bytes = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)));
            await queueClient.SendAsync(bytes);
        }
    }
}
