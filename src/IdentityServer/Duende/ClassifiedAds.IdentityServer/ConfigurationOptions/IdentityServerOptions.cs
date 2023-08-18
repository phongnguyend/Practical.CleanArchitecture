using CryptographyHelper.Certificates;

namespace ClassifiedAds.IdentityServer.ConfigurationOptions
{
    public class IdentityServerOptions : Duende.IdentityServer.Configuration.IdentityServerOptions
    {
        public CertificateOption Certificate { get; set; }
    }
}
