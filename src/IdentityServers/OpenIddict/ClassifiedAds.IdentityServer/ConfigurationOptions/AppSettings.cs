using ClassifiedAds.IdentityServer.ConfigurationOptions.ExternalLogin;
using ClassifiedAds.Infrastructure.Caching;
using ClassifiedAds.Infrastructure.Interceptors;
using ClassifiedAds.Infrastructure.Logging;
using ClassifiedAds.Infrastructure.Monitoring;
using System.Collections.Generic;

namespace ClassifiedAds.IdentityServer.ConfigurationOptions;

public class AppSettings
{
    public ConnectionStrings ConnectionStrings { get; set; }

    public IdentityServerOptions IdentityServer { get; set; }

    public LoggingOptions Logging { get; set; }

    public CachingOptions Caching { get; set; }

    public MonitoringOptions Monitoring { get; set; }

    public Dictionary<string, string> SecurityHeaders { get; set; }

    public InterceptorsOptions Interceptors { get; set; }

    public ExternalLoginOptions ExternalLogin { get; set; }

    public CookiePolicyOptions CookiePolicyOptions { get; set; }
}
