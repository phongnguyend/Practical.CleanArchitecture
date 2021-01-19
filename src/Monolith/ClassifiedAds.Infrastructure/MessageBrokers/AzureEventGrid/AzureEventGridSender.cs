using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.MessageBrokers.AzureEventGrid
{
    public class AzureEventGridSender<T> : IMessageSender<T>
    {
        private readonly string _domainEndpoint;
        private readonly string _domainKey;
        private readonly string _topic;

        public AzureEventGridSender(string domainEndpoint, string domainKey, string topic)
        {
            _domainEndpoint = new Uri(domainEndpoint).Host;
            _domainKey = domainKey;
            _topic = topic;
        }

        public async Task SendAsync(T message, MetaData metaData, CancellationToken cancellationToken = default)
        {
            TopicCredentials domainKeyCredentials = new TopicCredentials(_domainKey);
            EventGridClient client = new EventGridClient(domainKeyCredentials);

            var events = new List<EventGridEvent>()
            {
                new EventGridEvent
                {
                    Id = Guid.NewGuid().ToString(),
                    EventType = typeof(T).FullName,
                    Topic = _topic,
                    Data = new Message<T>
                    {
                        Data = message,
                        MetaData = metaData,
                    },
                    EventTime = DateTime.UtcNow,
                    Subject = "TEST",
                    DataVersion = "1.0",
                },
            };

            await client.PublishEventsAsync(_domainEndpoint, events, cancellationToken);
        }
    }
}
