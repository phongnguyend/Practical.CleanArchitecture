using ClassifiedAds.Infrastructure.Caching;
using ClassifiedAds.Infrastructure.Interceptors;
using ClassifiedAds.Infrastructure.Logging;
using ClassifiedAds.Infrastructure.MessageBrokers;
using ClassifiedAds.Infrastructure.Monitoring;
using ClassifiedAds.Infrastructure.Storages;
using CryptographyHelper.Certificates;

namespace ClassifiedAds.Services.Product.ConfigurationOptions;

public class AppSettings
{
    public ConnectionStrings ConnectionStrings { get; set; }

    public LoggingOptions Logging { get; set; }

    public CachingOptions Caching { get; set; }

    public MonitoringOptions Monitoring { get; set; }

    public IdentityServerAuthentication IdentityServerAuthentication { get; set; }

    public StorageOptions Storage { get; set; }

    public MessageBrokerOptions MessageBroker { get; set; }

    public InterceptorsOptions Interceptors { get; set; }
}

public class ConnectionStrings
{
    public string ClassifiedAds { get; set; }

    public string MigrationsAssembly { get; set; }
}

public class IdentityServerAuthentication
{
    public string Provider { get; set; }

    public string Authority { get; set; }

    public string ApiName { get; set; }

    public bool RequireHttpsMetadata { get; set; }

    public OpenIddictOptions OpenIddict { get; set; }
}

public class OpenIddictOptions
{
    public string IssuerUri { get; set; }

    public CertificateOption TokenDecryptionCertificate { get; set; }

    public CertificateOption IssuerSigningCertificate { get; set; }
}
