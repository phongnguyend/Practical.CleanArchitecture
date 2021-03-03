using ClassifiedAds.Infrastructure.Caching;
using ClassifiedAds.Infrastructure.DistributedTracing;
using ClassifiedAds.Infrastructure.Interceptors;
using ClassifiedAds.Infrastructure.Logging;
using ClassifiedAds.Infrastructure.Monitoring;

namespace ClassifiedAds.Services.Configuration.ConfigurationOptions
{
    public class AppSettings
    {
        public ConnectionStrings ConnectionStrings { get; set; }

        public LoggingOptions Logging { get; set; }

        public CachingOptions Caching { get; set; }

        public MonitoringOptions Monitoring { get; set; }

        public DistributedTracingOptions DistributedTracing { get; set; }

        public IdentityServerAuthentication IdentityServerAuthentication { get; set; }

        public InterceptorsOptions Interceptors { get; set; }

        public CertificatesOptions Certificates { get; set; }
    }

    public class ConnectionStrings
    {
        public string ClassifiedAds { get; set; }

        public string MigrationsAssembly { get; set; }
    }

    public class IdentityServerAuthentication
    {
        public string Authority { get; set; }

        public string ApiName { get; set; }

        public bool RequireHttpsMetadata { get; set; }
    }
}
