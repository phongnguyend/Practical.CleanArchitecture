using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using System.Collections.Generic;
using System.Net.Http;

namespace ClassifiedAds.Infrastructure.MessageBrokers.AzureEventGrid
{
    public static class AzureEventGridEventParser
    {
        public static IEnumerable<T> Parse<T>(HttpRequestMessage request)
        {
            string requestContent = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
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
                    yield return (T)eventGridEvent.Data;
                }
            }
        }
    }
}
