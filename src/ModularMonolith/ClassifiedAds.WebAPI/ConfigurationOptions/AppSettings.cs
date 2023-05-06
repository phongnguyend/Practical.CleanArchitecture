using ClassifiedAds.Infrastructure.Caching;
using ClassifiedAds.Infrastructure.Interceptors;
using ClassifiedAds.Infrastructure.Logging;
using ClassifiedAds.Infrastructure.Monitoring;
using System.Collections.Generic;

namespace ClassifiedAds.WebAPI.ConfigurationOptions;

public class AppSettings
{
    public LoggingOptions Logging { get; set; }

    public CachingOptions Caching { get; set; }

    public MonitoringOptions Monitoring { get; set; }

    public IdentityServerAuthentication IdentityServerAuthentication { get; set; }

    public string AllowedHosts { get; set; }

    public CORS CORS { get; set; }

    public Dictionary<string, string> SecurityHeaders { get; set; }

    public InterceptorsOptions Interceptors { get; set; }

    public ModulesOptions Modules { get; set; }
}
