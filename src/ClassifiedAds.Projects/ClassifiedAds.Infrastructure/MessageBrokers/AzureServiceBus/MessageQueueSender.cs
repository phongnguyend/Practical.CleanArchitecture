using ClassifiedAds.DomainServices.Infrastructure;

namespace ClassifiedAds.Infrastructure.MessageBrokers.AzureServiceBus
{
    public class MessageQueueSender<T> : IMessageQueueSender<T>
    {
        public void Send(T message, string exchangeName, string routingKey)
        {
            throw new System.NotImplementedException();
        }
    }
}
