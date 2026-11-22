using System.Text.Json;
using Sentry;

namespace ClassifiedAds.Mobile.Core;

public static class SentryReporting
{
    public static void Configure(MauiAppBuilder builder)
    {
#if !DEBUG
        using var stream = typeof(SentryReporting).Assembly.GetManifestResourceStream("sentry.config.json");
        if (stream == null)
        {
            return;
        }

        using var document = JsonDocument.Parse(stream);
        var config = document.RootElement;
        if (!config.TryGetProperty("SENTRY_ENABLED", out var enabled) || !enabled.GetBoolean() ||
            !config.TryGetProperty("SENTRY_DSN", out var dsn) || string.IsNullOrWhiteSpace(dsn.GetString()))
        {
            return;
        }

        var dsnValue = dsn.GetString();
        var environment = config.GetProperty("SENTRY_ENVIRONMENT").GetString();
        var release = config.GetProperty("SENTRY_RELEASE").GetString();
        builder.UseSentry(options =>
        {
            options.Dsn = dsnValue;
            options.Environment = environment;
            if (!string.IsNullOrWhiteSpace(release))
            {
                options.Release = release;
            }

            options.SendDefaultPii = false;
            options.MaxBreadcrumbs = 0;
            options.TracesSampleRate = 0;
            options.SetBeforeSend((sentryEvent, _) =>
            {
                sentryEvent.User = new SentryUser();
                sentryEvent.Request = new SentryRequest();
                sentryEvent.Message = null;
                if (sentryEvent.SentryExceptions != null)
                {
                    foreach (var exception in sentryEvent.SentryExceptions)
                    {
                        exception.Value = exception.Type ?? "Unexpected error";
                    }
                }

                return sentryEvent;
            });
        });
#endif
    }

    public static void Record(Exception exception)
    {
        if (SentrySdk.IsEnabled && exception is not OperationCanceledException)
        {
            SentrySdk.CaptureException(exception);
        }
    }
}
