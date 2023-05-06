using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Domain.Notification;

public interface IWebNotification<T>
{
    Task SendAsync(T message, CancellationToken cancellationToken = default);
}
