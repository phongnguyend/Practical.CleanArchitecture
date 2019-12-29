using ClassifiedAds.DomainServices.Infrastructure.MessageBrokers;

namespace ClassifiedAds.Infrastructure.MessageBrokers.AzureServiceBus
{
    public class AzureServiceBusSender : IMessageSender
    {
        public void Send<T>(T message)
        {
            throw new System.NotImplementedException();
        }
    }
}
