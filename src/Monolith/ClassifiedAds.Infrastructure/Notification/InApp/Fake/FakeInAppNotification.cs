using ClassifiedAds.Domain.Notification;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Notification.InApp.Fake;

public class FakeInAppNotification<T> : IInAppNotification<T>
{
    public Task SendAsync(T message, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
