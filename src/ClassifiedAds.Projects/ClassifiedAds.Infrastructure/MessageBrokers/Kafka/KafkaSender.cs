using ClassifiedAds.DomainServices.Infrastructure.MessageBrokers;

namespace ClassifiedAds.Infrastructure.MessageBrokers.Kafka
{
    public class KafkaSender : IMessageSender
    {
        public void Send<T>(T message)
        {
            throw new System.NotImplementedException();
        }
    }
}
