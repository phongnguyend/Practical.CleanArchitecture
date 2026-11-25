using FirebaseAdmin.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Notification.MobilePush;

public sealed class MobilePushDeliveryWorker : BackgroundService
{
    private readonly IServiceScopeFactory scopes;
    private readonly ILogger<MobilePushDeliveryWorker> logger;

    public MobilePushDeliveryWorker(IServiceScopeFactory scopes, ILogger<MobilePushDeliveryWorker> logger)
    {
        this.scopes = scopes;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopes.CreateScope();
                await DeliverAsync(scope.ServiceProvider.GetRequiredService<MobilePushStore>(), scope.ServiceProvider.GetRequiredService<IMobilePushSender>(), stoppingToken);
            }
            catch (Exception error) when (error is not OperationCanceledException)
            {
                // Never log tokens, payloads, credentials, or provider exception messages.
                logger.LogWarning("Push delivery cycle failed ({ErrorType}).", error.GetType().Name);
            }
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    public static async Task DeliverAsync(MobilePushStore db, IMobilePushSender sender, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var pending = await db.Deliveries.AsNoTracking().Where(x => x.CompletedAt == null && x.NextAttemptAt <= now).OrderBy(x => x.NextAttemptAt).Take(50).ToListAsync(cancellationToken);
        foreach (var item in pending)
        {
            // Atomic lease supports multiple workers. A crash may redeliver after lease expiry;
            // clients deduplicate by notificationId (at-least-once delivery).
            var claimed = await db.Deliveries.Where(x => x.Id == item.Id && x.CompletedAt == null && x.NextAttemptAt <= now)
                .ExecuteUpdateAsync(x => x.SetProperty(d => d.NextAttemptAt, now.AddMinutes(5)).SetProperty(d => d.Attempts, d => d.Attempts + 1), cancellationToken);
            if (claimed == 0)
            {
                continue;
            }
            var device = await db.Devices.AsNoTracking().SingleOrDefaultAsync(x => x.Id == item.DeviceId && x.UserId == item.UserId, cancellationToken);
            var message = await db.Messages.AsNoTracking().SingleOrDefaultAsync(x => x.Id == item.MessageId && x.UserId == item.UserId, cancellationToken);
            string errorCode = null;
            var completed = device == null || message == null;
            if (!completed)
            {
                try
                {
                    using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                    timeout.CancelAfter(TimeSpan.FromSeconds(30));
                    await sender.SendAsync(device, message, timeout.Token);
                    completed = true;
                }
                catch (FirebaseMessagingException error) when (error.MessagingErrorCode == MessagingErrorCode.Unregistered)
                {
                    await db.Devices.Where(x => x.Id == device.Id && x.Token == device.Token && x.UserId == item.UserId).ExecuteDeleteAsync(cancellationToken);
                    completed = true;
                    errorCode = "Unregistered";
                }
                catch (Exception error) when (!cancellationToken.IsCancellationRequested)
                {
                    errorCode = error is FirebaseMessagingException firebase ? firebase.MessagingErrorCode?.ToString() : "DeliveryFailed";
                    completed = item.Attempts >= 7;
                }
            }
            await db.Deliveries.Where(x => x.Id == item.Id).ExecuteUpdateAsync(x => x
                .SetProperty(d => d.CompletedAt, completed ? DateTime.UtcNow : (DateTime?)null)
                .SetProperty(d => d.LastError, errorCode)
                .SetProperty(d => d.NextAttemptAt, DateTime.UtcNow.AddSeconds(Math.Min(3600, 30 * Math.Pow(2, item.Attempts)))), cancellationToken);
        }
    }
}
