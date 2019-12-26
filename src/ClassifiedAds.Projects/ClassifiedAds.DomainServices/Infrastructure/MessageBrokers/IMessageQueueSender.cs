namespace ClassifiedAds.DomainServices.Infrastructure.MessageBrokers
{
    public interface IMessageQueueSender<T>
    {
        void Send(T message, string exchangeName, string routingKey);
    }
}
