namespace ClassifiedAds.DomainServices.Infrastructure
{
    public interface IMessageQueueSender<T>
    {
        void Send(T message, string exchangeName, string routingKey);
    }
}
