using System;

namespace ClassifiedAds.DomainServices.Infrastructure.MessageBrokers
{
    public interface IMessageReceiver
    {
        void Receive<T>(Action<T> action);
    }
}
