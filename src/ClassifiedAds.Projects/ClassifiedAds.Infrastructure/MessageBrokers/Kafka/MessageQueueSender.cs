using ClassifiedAds.DomainServices.Infrastructure.MessageBrokers;

namespace ClassifiedAds.Infrastructure.MessageBrokers.Kafka
{
    public class MessageQueueSender<T> : IMessageQueueSender<T>
    {
        public void Send(T message, string exchangeName, string routingKey)
        {
            throw new System.NotImplementedException();
        }
    }
}
