using System;

namespace ClassifiedAds.Modules.MessageBrokers.Contracts.Services
{
    public interface IMessageBusReceiver<T>
    {
        void Receive(Action<T> action);
    }
}
