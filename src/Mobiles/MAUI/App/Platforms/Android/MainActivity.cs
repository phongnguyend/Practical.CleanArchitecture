using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Content;

namespace ClassifiedAds.Mobile;

[Activity(Name = "com.classifiedads.mobile.maui.MainActivity", Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
#if FIREBASE_MESSAGING
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        if (OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            var manager = (NotificationManager)GetSystemService(NotificationService)!;
            manager.CreateNotificationChannel(new NotificationChannel("classifiedads_updates", "Classified Ads updates", NotificationImportance.Default));
        }

        Plugin.Firebase.CloudMessaging.FirebaseCloudMessagingImplementation.ChannelId = "classifiedads_updates";
        Plugin.Firebase.CloudMessaging.FirebaseCloudMessagingImplementation.OnNewIntent(Intent);
    }

    protected override void OnNewIntent(Intent? intent)
    {
        base.OnNewIntent(intent);
        Plugin.Firebase.CloudMessaging.FirebaseCloudMessagingImplementation.OnNewIntent(intent);
    }
#endif
}
