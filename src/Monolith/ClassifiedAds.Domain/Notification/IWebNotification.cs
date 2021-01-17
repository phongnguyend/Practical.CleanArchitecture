using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Domain.Notification
{
    public interface IWebNotification<T>
    {
        void Send(T message);

        Task SendAsync(T message, CancellationToken cancellationToken = default);
    }
}
