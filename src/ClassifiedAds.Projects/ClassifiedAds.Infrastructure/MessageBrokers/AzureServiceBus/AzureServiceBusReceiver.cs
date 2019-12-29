using ClassifiedAds.DomainServices.Infrastructure.MessageBrokers;
using System;

namespace ClassifiedAds.Infrastructure.MessageBrokers.AzureServiceBus
{
    public class AzureServiceBusReceiver : IMessageReceiver
    {
        public void Receive<T>(Action<T> action)
        {
            throw new NotImplementedException();
        }
    }
}
