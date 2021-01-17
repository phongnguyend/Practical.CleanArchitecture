using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Notification.Web
{
    public interface IWebNotification<T>
    {
        void Send(T message);

        Task SendAsync(T message, CancellationToken cancellationToken = default);
    }
}
