using ClassifiedAds.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Notification.MobilePush;

public sealed class FakeMobilePushSender : IMobilePushSender
{
    public Task SendAsync(MobilePushDevice device, MobilePushMessage message, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.CompletedTask;
    }
}
