using CryptographyHelper.Certificates;

namespace ClassifiedAds.WebAPI.ConfigurationOptions;

public class AuthenticationOptions
{
    public string Provider { get; set; }

    public IdentityServerOptions IdentityServer { get; set; }

    public JwtOptions Jwt { get; set; }
}

public class IdentityServerOptions
{
    public string Authority { get; set; }

    public string Audience { get; set; }

    public bool RequireHttpsMetadata { get; set; }
}

public class JwtOptions
{
    public string IssuerUri { get; set; }

    public string Audience { get; set; }

    public CertificateOption TokenDecryptionCertificate { get; set; }

    public CertificateOption IssuerSigningCertificate { get; set; }
}
