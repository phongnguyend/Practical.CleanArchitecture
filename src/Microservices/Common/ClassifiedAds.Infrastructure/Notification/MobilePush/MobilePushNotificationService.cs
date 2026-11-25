using ClassifiedAds.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Notification.MobilePush;

public interface IMobilePushNotificationService
{
    Task<Guid?> EnqueueAsync(string userId, string title, string body, string section = "notifications", CancellationToken cancellationToken = default);
}

public sealed class MobilePushNotificationService : IMobilePushNotificationService
{
    public static readonly string[] Sections = { "home", "products", "files", "users", "roles", "configurationentries", "auditlogentries", "notifications" };
    private readonly MobilePushStore db;
    private readonly MobilePushOptions options;

    public MobilePushNotificationService(MobilePushStore db, IOptions<MobilePushOptions> options)
    {
        this.db = db;
        this.options = options.Value;
    }

    public async Task<Guid?> EnqueueAsync(string userId, string title, string body, string section = "notifications", CancellationToken cancellationToken = default)
    {
        if (!options.UsedFirebase() && !options.UsedFake())
        {
            throw new InvalidOperationException("Unsupported mobile push provider.");
        }
        if (string.IsNullOrWhiteSpace(userId) || userId.Length > 200 || string.IsNullOrWhiteSpace(title) || title.Length > 100 || string.IsNullOrWhiteSpace(body) || body.Length > 500 || !Sections.Contains(section, StringComparer.Ordinal))
        {
            throw new ArgumentException("Invalid push notification.");
        }
        var message = new MobilePushMessage { Id = Guid.NewGuid(), UserId = userId, Title = title, Body = body, Section = section, CreatedAt = DateTime.UtcNow };
        var devices = await db.Devices.Where(x => x.UserId == userId).Select(x => x.Id).ToListAsync(cancellationToken);
        db.Messages.Add(message);
        foreach (var device in devices)
        {
            db.Deliveries.Add(new MobilePushDelivery { Id = Guid.NewGuid(), MessageId = message.Id, DeviceId = device, UserId = userId, NextAttemptAt = message.CreatedAt });
        }
        // Inbox and delivery queue commit together.
        await db.SaveChangesAsync(cancellationToken);
        return message.Id;
    }
}
