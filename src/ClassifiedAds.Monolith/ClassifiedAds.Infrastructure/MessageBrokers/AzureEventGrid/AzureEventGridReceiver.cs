using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.MessageBrokers.AzureEventGrid
{
    public class AzureEventGridReceiver<T> : IMessageReceiver<T>
    {
        private readonly HttpRequestMessage _request;

        public AzureEventGridReceiver(HttpRequestMessage request)
        {
            _request = request;
        }

        public void Receive(Action<T> action)
        {
            ReceiveAsync(action).GetAwaiter().GetResult();
        }

        public async Task ReceiveAsync(Action<T> action)
        {
            string requestContent = await _request.Content.ReadAsStringAsync();
            EventGridSubscriber eventGridSubscriber = new EventGridSubscriber();
            eventGridSubscriber.AddOrUpdateCustomEventMapping(typeof(T).FullName, typeof(T));
            EventGridEvent[] eventGridEvents = eventGridSubscriber.DeserializeEventGridEvents(requestContent);
            foreach (EventGridEvent eventGridEvent in eventGridEvents)
            {
                if (eventGridEvent.Data is SubscriptionValidationEventData)
                {
                    var eventData = (SubscriptionValidationEventData)eventGridEvent.Data;
                }
                else if (eventGridEvent.Data.GetType() == typeof(T))
                {
                    var eventData = (T)eventGridEvent.Data;
                    action(eventData);
                }
            }
        }
    }
}
