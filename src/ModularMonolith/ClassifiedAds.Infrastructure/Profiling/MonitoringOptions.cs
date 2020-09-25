namespace ClassifiedAds.Infrastructure.Profiling
{
    public class MonitoringOptions
    {
        public MiniProfilerOptions MiniProfiler { get; set; }

        public AzureApplicationInsightsOptions AzureApplicationInsights { get; set; }
    }

    public class MiniProfilerOptions
    {
        public bool IsEnabled { get; set; }

        public SqlServerStorageOptions SqlServerStorage { get; set; }

        public class SqlServerStorageOptions
        {
            public string ConectionString { get; set; }

            public string ProfilersTable { get; set; }

            public string TimingsTable { get; set; }

            public string ClientTimingsTable { get; set; }
        }
    }

    public class AzureApplicationInsightsOptions
    {
        public bool IsEnabled { get; set; }

        public string InstrumentationKey { get; set; }

        public bool EnableSqlCommandTextInstrumentation { get; set; }
    }
}
