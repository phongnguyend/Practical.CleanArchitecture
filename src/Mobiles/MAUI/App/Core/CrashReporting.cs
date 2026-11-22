namespace ClassifiedAds.Mobile.Core;

public static class CrashReporting
{
    private static bool enabled = false;

    public static void Initialize()
    {
#if FIREBASE_CRASHLYTICS
        if (enabled)
        {
            return;
        }

        var client = Plugin.Firebase.Crashlytics.CrossFirebaseCrashlytics.Current;
        client.SetCustomKey("framework", "MAUI");
        client.SetCrashlyticsCollectionEnabled(true);
        enabled = true;
        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
        {
            if (args.ExceptionObject is Exception exception)
            {
                Record(exception, forwardToSentry: false);
            }
        };
        TaskScheduler.UnobservedTaskException += (_, args) => Record(args.Exception, forwardToSentry: false);
#endif
    }

    public static void Record(Exception exception, bool forwardToSentry = true)
    {
        if (forwardToSentry)
        {
            SentryReporting.Record(exception);
        }

        if (!enabled || exception is OperationCanceledException)
        {
            return;
        }

#if FIREBASE_CRASHLYTICS
        // Do not attach API response bodies, credentials, or exception messages.
        Plugin.Firebase.Crashlytics.CrossFirebaseCrashlytics.Current.RecordException(new SafeException(exception));
#endif
    }

#if FIREBASE_CRASHLYTICS
    private sealed class SafeException(Exception original) : Exception(original.GetType().Name)
    {
        public override string? StackTrace => original.StackTrace;
    }
#endif
}
