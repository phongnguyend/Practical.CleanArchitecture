using ClassifiedAds.DomainServices.Infrastructure;
using System;

namespace ClassifiedAds.Infrastructure.MessageBrokers.AzureQueue
{
    public class MessageQueueReceiver<T> : IMessageQueueReceiver<T>
    {
        public void Receive(Action<T> action)
        {
            throw new NotImplementedException();
        }
    }
}
