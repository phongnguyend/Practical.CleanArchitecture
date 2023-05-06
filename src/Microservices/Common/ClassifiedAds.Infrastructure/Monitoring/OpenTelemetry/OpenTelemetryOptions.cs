namespace ClassifiedAds.Infrastructure.Monitoring.OpenTelemetry;

public class OpenTelemetryOptions
{
    public bool IsEnabled { get; set; }

    public string ServiceName { get; set; }

    public ZipkinOptions Zipkin { get; set; }

    public JaegerOptions Jaeger { get; set; }

    public OtlpOptions Otlp { get; set; }
}

public class ZipkinOptions
{
    public bool IsEnabled { get; set; }

    public string Endpoint { get; set; }
}

public class JaegerOptions
{
    public bool IsEnabled { get; set; }

    public string AgentHost { get; set; }

    public int AgentPort { get; set; }
}

public class OtlpOptions
{
    public bool IsEnabled { get; set; }

    public string Endpoint { get; set; }
}
