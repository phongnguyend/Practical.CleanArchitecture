using ClassifiedAds.DomainServices.Infrastructure.MessageBrokers;

namespace ClassifiedAds.Infrastructure.MessageBrokers.Kafka
{
    public class KafkaSender<T> : IMessageSender<T>
    {
        public void Send(T message)
        {
            throw new System.NotImplementedException();
        }
    }
}
