using ClassifiedAds.DomainServices.Infrastructure.MessageBrokers;
using System;

namespace ClassifiedAds.Infrastructure.MessageBrokers.AzureQueue
{
    public class AzureQueueReceiver : IMessageReceiver
    {
        public void Receive<T>(Action<T> action)
        {
            throw new NotImplementedException();
        }
    }
}
