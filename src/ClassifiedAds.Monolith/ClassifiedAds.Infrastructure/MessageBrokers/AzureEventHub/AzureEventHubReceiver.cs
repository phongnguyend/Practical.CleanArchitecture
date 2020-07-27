using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using System;

namespace ClassifiedAds.Infrastructure.MessageBrokers.AzureEventHub
{
    public class AzureEventHubReceiver<T> : IMessageReceiver<T>
    {
        public AzureEventHubReceiver()
        {
        }

        public void Receive(Action<T> action)
        {
        }
    }
}
