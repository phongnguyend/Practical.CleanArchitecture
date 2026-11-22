using Microsoft.Maui.LifecycleEvents;

namespace ClassifiedAds.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder().UseMauiApp<App>().ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        });
        Core.SentryReporting.Configure(builder);
#if FIREBASE_CRASHLYTICS
        builder.ConfigureLifecycleEvents(events =>
        {
#if ANDROID
            events.AddAndroid(android => android.OnCreate((activity, _) =>
            {
                Plugin.Firebase.Core.Platforms.Android.CrossFirebase.Initialize(activity, () => Platform.CurrentActivity!);
                Core.CrashReporting.Initialize();
            }));
#elif IOS
            events.AddiOS(ios => ios.WillFinishLaunching((_, _) =>
            {
                Plugin.Firebase.Core.Platforms.iOS.CrossFirebase.Initialize();
                Core.CrashReporting.Initialize();
                return true;
            }));
#endif
        });
#endif
        return builder.Build();
    }
}
