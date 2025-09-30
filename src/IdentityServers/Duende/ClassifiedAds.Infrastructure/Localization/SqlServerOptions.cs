namespace ClassifiedAds.Infrastructure.Localization;

public class SqlServerOptions
{
    public bool IsEnabled { get; set; }

    public string ConnectionString { get; set; }

    public string SqlQuery { get; set; }

    public int CacheMinutes { get; set; }
}
