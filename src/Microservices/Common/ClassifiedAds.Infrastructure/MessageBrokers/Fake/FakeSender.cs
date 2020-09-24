using ClassifiedAds.Domain.Infrastructure.MessageBrokers;

namespace ClassifiedAds.Infrastructure.MessageBrokers.Fake
{
    public class FakeSender<T> : IMessageSender<T>
    {
        public void Send(T message)
        {
            // do nothing
        }
    }
}
