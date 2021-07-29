using CryptographyHelper.Certificates;

namespace ClassifiedAds.IdentityServer.ConfigurationOptions
{
    public class IdentityServerOptions : IdentityServer4.Configuration.IdentityServerOptions
    {
        public CertificateOption Certificate { get; set; }
    }
}
