using System;

namespace ClassifiedAds.DomainServices.Infrastructure.MessageBrokers
{
    public interface IMessageQueueReceiver<T>
    {
        void Receive(Action<T> action);
    }
}
