using ClassifiedAds.DomainServices.Infrastructure.MessageBrokers;
using System;

namespace ClassifiedAds.Infrastructure.MessageBrokers.Kafka
{
    public class KafkaReceiver : IMessageReceiver
    {
        public void Receive<T>(Action<T> action)
        {
            throw new NotImplementedException();
        }
    }
}
