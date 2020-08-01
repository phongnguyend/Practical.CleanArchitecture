namespace ClassifiedAds.Domain.Infrastructure.MessageBrokers
{
    public interface IMessageSender<T>
    {
        void Send(T message);
    }
}
