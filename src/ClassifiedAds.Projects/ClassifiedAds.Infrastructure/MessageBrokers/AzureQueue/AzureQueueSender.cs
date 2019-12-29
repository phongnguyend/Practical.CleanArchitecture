using ClassifiedAds.DomainServices.Infrastructure.MessageBrokers;

namespace ClassifiedAds.Infrastructure.MessageBrokers.AzureQueue
{
    public class AzureQueueSender : IMessageSender
    {
        public void Send<T>(T message)
        {
            throw new System.NotImplementedException();
        }
    }
}
