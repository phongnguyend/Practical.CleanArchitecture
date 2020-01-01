namespace ClassifiedAds.DomainServices.Infrastructure.MessageBrokers
{
    public interface IMessageSender<T>
    {
        void Send(T message);
    }
}
