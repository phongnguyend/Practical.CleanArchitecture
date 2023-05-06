namespace ClassifiedAds.Infrastructure.Logging;

public class ApplicationInsightsOptions
{
    public bool IsEnabled { get; set; }

    public string InstrumentationKey { get; set; }
}
