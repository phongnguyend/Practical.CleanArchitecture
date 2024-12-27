namespace ClassifiedAds.Infrastructure.Monitoring.MiniProfiler;

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
