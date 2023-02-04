namespace ClassifiedAds.Infrastructure.Monitoring.AzureApplicationInsights
{
    public class AzureApplicationInsightsOptions
    {
        public bool IsEnabled { get; set; }

        public string InstrumentationKey { get; set; }

        public bool EnableSqlCommandTextInstrumentation { get; set; }
    }
}
