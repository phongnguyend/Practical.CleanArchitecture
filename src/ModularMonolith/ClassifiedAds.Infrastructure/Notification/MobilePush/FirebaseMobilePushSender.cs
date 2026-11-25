using ClassifiedAds.Domain.Entities;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Notification.MobilePush;

public interface IMobilePushSender
{
    Task SendAsync(MobilePushDevice device, MobilePushMessage message, CancellationToken cancellationToken);
}

public sealed class FirebaseMobilePushSender : IMobilePushSender, IDisposable
{
    private readonly Lazy<FirebaseApp> app;

    public FirebaseMobilePushSender(IOptions<MobilePushOptions> options)
    {
        // ADC: workload identity or GOOGLE_APPLICATION_CREDENTIALS on the server only.
        app = new Lazy<FirebaseApp>(() => FirebaseApp.Create(new AppOptions
        {
            ProjectId = options.Value.Firebase.ProjectId,
            Credential = GoogleCredential.GetApplicationDefault(),
        }, "classifiedads-push-" + Guid.NewGuid()));
    }

    public Task SendAsync(MobilePushDevice device, MobilePushMessage message, CancellationToken cancellationToken)
    {
        return FirebaseMessaging.GetMessaging(app.Value).SendAsync(new Message
        {
            Token = device.Token,
            // Generic lock-screen text; private inbox content is fetched after authentication/unlock.
            Notification = new FirebaseAdmin.Messaging.Notification { Title = "Classified Ads", Body = "You have a new notification." },
            Data = new Dictionary<string, string> { ["notificationId"] = message.Id.ToString(), ["section"] = message.Section },
            Android = new AndroidConfig { Priority = Priority.High, TimeToLive = TimeSpan.FromHours(24), Notification = new AndroidNotification { ChannelId = "classifiedads_updates", Tag = message.Id.ToString() } },
            Apns = new ApnsConfig { Aps = new Aps { Sound = "default" } },
        }, cancellationToken);
    }

    public void Dispose()
    {
        if (app.IsValueCreated)
        {
            app.Value.Delete();
        }
    }
}
