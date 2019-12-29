namespace ClassifiedAds.DomainServices.Infrastructure.MessageBrokers
{
    public interface IMessageSender
    {
        void Send<T>(T message);
    }
}
