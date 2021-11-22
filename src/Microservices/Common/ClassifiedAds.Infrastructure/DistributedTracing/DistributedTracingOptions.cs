namespace ClassifiedAds.Infrastructure.DistributedTracing
{
    public class DistributedTracingOptions
    {
        public bool IsEnabled { get; set; }

        public ExporterOptions Exporter { get; set; }

        public ZipkinOptions Zipkin { get; set; }

        public JaegerOptions Jaeger { get; set; }

        public OtlpOptions Otlp { get; set; }
    }

    public enum ExporterOptions
    {
        Zipkin,
        Jaeger,
        Otlp,
    }

    public class ZipkinOptions
    {
        public string ServiceName { get; set; }

        public string Endpoint { get; set; }
    }

    public class JaegerOptions
    {
        public string ServiceName { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }
    }

    public class OtlpOptions
    {
        public string ServiceName { get; set; }

        public string Endpoint { get; set; }
    }
}
