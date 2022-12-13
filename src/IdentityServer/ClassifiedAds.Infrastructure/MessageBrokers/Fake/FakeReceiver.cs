using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using System;

namespace ClassifiedAds.Infrastructure.MessageBrokers.Fake
{
    public class FakeReceiver<T> : IMessageReceiver<T>
    {
        public void Receive(Action<T, MetaData> action)
        {
            // do nothing
        }
    }
}
