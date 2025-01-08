namespace ClassifiedAds.Infrastructure.Monitoring.OpenTelemetry;

public class OpenTelemetryOptions
{
    public bool IsEnabled { get; set; }

    public string ServiceName { get; set; }

    public bool TraceEnabled { get; set; }

    public bool MetricEnabled { get; set; }

    public ZipkinOptions Zipkin { get; set; }

    public OtlpOptions Otlp { get; set; }

    public AzureMonitorOptions AzureMonitor { get; set; }
}

public class ZipkinOptions
{
    public bool IsEnabled { get; set; }

    public string Endpoint { get; set; }
}

public class OtlpOptions
{
    public bool IsEnabled { get; set; }

    public string Endpoint { get; set; }
}

public class AzureMonitorOptions
{
    public bool IsEnabled { get; set; }

    public string ConnectionString { get; set; }
}
