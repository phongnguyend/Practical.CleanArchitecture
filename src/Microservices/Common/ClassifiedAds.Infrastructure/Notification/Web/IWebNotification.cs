using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Notification.Web
{
    public interface IWebNotification
    {
        void Send<T>(string endpoint, string eventName, T message);
        Task SendAsync<T>(string endpoint, string eventName, T message);
    }
}
