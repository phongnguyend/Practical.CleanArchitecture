using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace ClassifiedAds.Infrastructure.Monitoring.AzureApplicationInsights;

public class CustomTelemetryProcessor : ITelemetryProcessor
{
    private ITelemetryProcessor Next { get; set; }

    public CustomTelemetryProcessor(ITelemetryProcessor next)
    {
        Next = next;
    }

    public void Process(ITelemetry item)
    {
        if (!OKtoSend(item))
        {
            return;
        }

        Next.Process(item);
    }

    private static bool OKtoSend(ITelemetry item)
    {
        if (item is RequestTelemetry request)
        {
        }

        if (item is DependencyTelemetry dependency)
        {
        }

        return true;
    }
}
