using System;

namespace ClassifiedAds.DomainServices.Infrastructure.MessageBrokers
{
    public interface IMessageReceiver<T>
    {
        void Receive(Action<T> action);
    }
}
