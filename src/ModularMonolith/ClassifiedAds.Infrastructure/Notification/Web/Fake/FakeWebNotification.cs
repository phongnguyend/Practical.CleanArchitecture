using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Notification.Web.Fake
{
    public class FakeWebNotification : IWebNotification
    {
        public void Send<T>(string endpoint, string eventName, T message)
        {
            // do nothing
        }

        public Task SendAsync<T>(string endpoint, string eventName, T message)
        {
            // do nothing
            return Task.CompletedTask;
        }
    }
}
