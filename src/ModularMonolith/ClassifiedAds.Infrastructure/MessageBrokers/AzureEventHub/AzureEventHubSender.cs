﻿using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using Microsoft.Azure.EventHubs;
using Newtonsoft.Json;
using System.Text;
using System.Threading;
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

        public async Task SendAsync(T message, MetaData metaData, CancellationToken cancellationToken = default)
        {
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(_connectionString)
            {
                EntityPath = _hubName,
            };

            var eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());

            await eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new Message<T>
            {
                Data = message,
                MetaData = metaData,
            }))));

            await eventHubClient.CloseAsync();
        }
    }
}
