using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace ClassifiedAds.Infrastructure.Monitoring.AzureApplicationInsights;

public class CustomTelemetryInitializer : ITelemetryInitializer
{
    public void Initialize(ITelemetry telemetry)
    {
        if (!(telemetry is RequestTelemetry requestTelemetry))
        {
            return;
        }

        if (int.TryParse(requestTelemetry.ResponseCode, out int code))
        {
            if (code >= 400 && code < 500)
            {
                // If we set the Success property, the SDK won't change it:
                requestTelemetry.Success = true;

                // Allow us to filter these requests in the portal:
                requestTelemetry.Properties["Overridden400s"] = "true";
            }
        }
    }
}
