using System.Text.Json;
using ClassifiedAds.Mobile.Core.Notifications;
#if FIREBASE_MESSAGING
using Plugin.Firebase.CloudMessaging;
#endif

namespace ClassifiedAds.Mobile;

public sealed class PushNotifications(AppServices services)
{
#if FIREBASE_MESSAGING
    public static bool Configured => true;
#else
    public static bool Configured => false;
#endif
    private (string Id, string Section)? pending;
    private int generation;
    private bool initialized;

    public event Action? Opened;

    public event Action? Received;

    public void Initialize()
    {
        if (!Configured || initialized)
        {
            return;
        }

        initialized = true;
#if FIREBASE_MESSAGING
        var client = CrossFirebaseCloudMessaging.Current;
        client.TokenChanged += async (_, args) =>
        {
            try
            {
                await Register(args.Token);
            }
            catch
            {
                // Retry on the next authenticated foreground session.
            }
        };
        client.NotificationReceived += (_, _) => MainThread.BeginInvokeOnMainThread(() =>
        {
            if (services.SignedIn)
            {
                Received?.Invoke();
            }
        });
        client.NotificationTapped += (_, args) => MainThread.BeginInvokeOnMainThread(() =>
        {
            var data = args.Notification.Data;
            if (data != null && data.TryGetValue("notificationId", out var id))
            {
                data.TryGetValue("section", out var section);
                var target = NotificationTarget.Parse(id, section);
                if (target != null)
                {
                    pending = (target.Id.ToString(), target.Section);
                    Opened?.Invoke();
                }
            }
        });
#endif
    }

    private async Task Register(string token)
    {
        if (!services.SignedIn || !Preferences.Get("push.enabled", false))
        {
            return;
        }

        var id = Preferences.Get("push.installation", "");
        if (string.IsNullOrEmpty(id))
        {
            id = Guid.NewGuid().ToString();
            Preferences.Set("push.installation", id);
        }

        await services.Api.Write($"notifications/devices/{id}", HttpMethod.Put, new { app = "MAUI", platform = DeviceInfo.Platform == DevicePlatform.iOS ? "ios" : "android", token });
    }

    public async Task Sync()
    {
        if (!Configured || !services.SignedIn || !Preferences.Get("push.enabled", false))
        {
            return;
        }

#if FIREBASE_MESSAGING
        var epoch = generation;
        var token = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
        if (epoch == generation && services.SignedIn)
        {
            await Register(token);
        }
#else
        await Task.CompletedTask;
#endif
    }

    public async Task Enable()
    {
        if (!Configured || !(await services.Api.Get<JsonElement>("notifications/status")).GetProperty("enabled").GetBoolean())
        {
            throw new InvalidOperationException("Push notifications are not configured.");
        }

#if FIREBASE_MESSAGING
        var epoch = generation;
#if ANDROID
        if (OperatingSystem.IsAndroidVersionAtLeast(33) && await Permissions.RequestAsync<NotificationPermission>() != PermissionStatus.Granted)
        {
            throw new InvalidOperationException("Allow notifications in device settings to receive updates.");
        }
#endif
        await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();
        if (epoch != generation || !services.SignedIn)
        {
            throw new InvalidOperationException("Unlock your workspace before enabling notifications.");
        }

        Preferences.Set("push.enabled", true);
#if ANDROID
        Firebase.Messaging.FirebaseMessaging.Instance.AutoInitEnabled = true;
#elif IOS
        Firebase.CloudMessaging.Messaging.SharedInstance.AutoInitEnabled = true;
#endif
        await Sync();
#endif
    }

    public async Task Disable()
    {
        generation++;
        pending = null;
        var id = Preferences.Get("push.installation", "");
        Preferences.Remove("push.enabled");
        Preferences.Remove("push.installation");
        try
        {
            if (id.Length > 0 && services.SignedIn)
            {
                await services.Api.Write($"notifications/devices/{id}", HttpMethod.Delete);
            }
        }
        finally
        {
#if FIREBASE_MESSAGING && ANDROID
            Firebase.Messaging.FirebaseMessaging.Instance.AutoInitEnabled = false;
            await Android.Gms.Extensions.TasksExtensions.AsAsync(Firebase.Messaging.FirebaseMessaging.Instance.DeleteToken());
#elif FIREBASE_MESSAGING && IOS
            Firebase.CloudMessaging.Messaging.SharedInstance.AutoInitEnabled = false;
            await Firebase.CloudMessaging.Messaging.SharedInstance.DeleteTokenAsync();
#endif
        }
    }

    public async Task<string?> Consume()
    {
        if (!services.SignedIn || pending is not { } target)
        {
            return null;
        }

        pending = null;
        try
        {
            await services.Api.Write($"notifications/inbox/{target.Id}/read", HttpMethod.Put);
        }
        catch
        {
            // The inbox can be refreshed after reconnecting.
        }

        return target.Section;
    }

#if ANDROID
    [System.Runtime.Versioning.SupportedOSPlatform("android33.0")]
    public sealed class NotificationPermission : Permissions.BasePlatformPermission
    {
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions => [(Android.Manifest.Permission.PostNotifications, true)];
    }
#endif
}
