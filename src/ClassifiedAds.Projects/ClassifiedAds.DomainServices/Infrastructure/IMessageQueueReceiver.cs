using System;

namespace ClassifiedAds.DomainServices.Infrastructure
{
    public interface IMessageQueueReceiver<T>
    {
        void Receive(Action<T> action);
    }
}
