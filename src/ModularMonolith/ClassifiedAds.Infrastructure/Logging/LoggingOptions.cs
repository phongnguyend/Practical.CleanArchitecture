using Serilog.Events;
using System.Collections.Generic;

namespace ClassifiedAds.Infrastructure.Logging;

public class LoggingOptions
{
    public Dictionary<string, string> LogLevel { get; set; }

    public FileOptions File { get; set; }

    public EventLogOptions EventLog { get; set; }

    public ApplicationInsightsOptions ApplicationInsights { get; set; }

    public OpenTelemetryOptions OpenTelemetry { get; set; }

    public class FileOptions
    {
        public LogEventLevel MinimumLogEventLevel { get; set; }
    }

    public class EventLogOptions
    {
        public bool IsEnabled { get; set; }

        public string LogName { get; set; }

        public string SourceName { get; set; }
    }

    public class ApplicationInsightsOptions
    {
        public bool IsEnabled { get; set; }

        public string InstrumentationKey { get; set; }
    }

    public class OpenTelemetryOptions
    {
        public bool IsEnabled { get; set; }

        public string ServiceName { get; set; }

        public OtlpOptions Otlp { get; set; }

        public AzureMonitorOptions AzureMonitor { get; set; }

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
    }
}
