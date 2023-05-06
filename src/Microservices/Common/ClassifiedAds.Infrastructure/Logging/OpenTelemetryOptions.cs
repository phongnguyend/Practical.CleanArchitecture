namespace ClassifiedAds.Infrastructure.Logging;

public class OpenTelemetryOptions
{
    public bool IsEnabled { get; set; }

    public string ServiceName { get; set; }

    public OtlpOptions Otlp { get; set; }
}

public class OtlpOptions
{
    public string Endpoint { get; set; }
}
