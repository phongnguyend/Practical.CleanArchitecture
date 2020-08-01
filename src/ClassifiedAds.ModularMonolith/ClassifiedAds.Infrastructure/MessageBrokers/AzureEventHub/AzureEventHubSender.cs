using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using Microsoft.Azure.EventHubs;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.MessageBrokers.AzureEventHub
{
    public class AzureEventHubSender<T> : IMessageSender<T>
    {
        private readonly string _connectionString;
        private readonly string _hubName;

        public AzureEventHubSender(string connectionString, string hubName)
        {
            _connectionString = connectionString;
            _hubName = hubName;
        }

        public void Send(T message)
        {
            SendAsync(message).GetAwaiter().GetResult();
        }

        private async Task SendAsync(T message)
        {
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(_connectionString)
            {
                EntityPath = _hubName,
            };

            var eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());

            await eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message))));

            await eventHubClient.CloseAsync();
        }
    }
}
