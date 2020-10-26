using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Notification.Web.Fake
{
    public class FakeWebNotification<T> : IWebNotification<T>
    {
        public void Send(T message)
        {
            // do nothing
        }

        public Task SendAsync(T message)
        {
            // do nothing
            return Task.CompletedTask;
        }
    }
}
