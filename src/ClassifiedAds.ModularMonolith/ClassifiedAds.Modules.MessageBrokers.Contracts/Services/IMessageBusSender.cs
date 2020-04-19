namespace ClassifiedAds.Modules.MessageBrokers.Contracts.Services
{
    public interface IMessageBusSender<T>
    {
        void Send(T message);
    }
}
