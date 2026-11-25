using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Notification.InApp;

public interface IInAppNotification<T>
{
    Task SendAsync(T message, CancellationToken cancellationToken = default);
}
