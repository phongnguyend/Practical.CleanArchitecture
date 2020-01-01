using ClassifiedAds.DomainServices.Infrastructure.MessageBrokers;
using System;

namespace ClassifiedAds.Infrastructure.MessageBrokers.Kafka
{
    public class KafkaReceiver<T> : IMessageReceiver<T>
    {
        public void Receive(Action<T> action)
        {
            throw new NotImplementedException();
        }
    }
}
