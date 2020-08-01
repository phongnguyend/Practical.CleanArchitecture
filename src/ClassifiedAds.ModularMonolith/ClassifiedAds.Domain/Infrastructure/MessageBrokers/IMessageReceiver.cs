using System;

namespace ClassifiedAds.Domain.Infrastructure.MessageBrokers
{
    public interface IMessageReceiver<T>
    {
        void Receive(Action<T> action);
    }
}
