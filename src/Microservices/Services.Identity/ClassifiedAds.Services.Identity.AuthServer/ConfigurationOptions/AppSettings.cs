using ClassifiedAds.IdentityServer.ConfigurationOptions.ExternalLogin;
using System.Collections.Generic;

namespace ClassifiedAds.IdentityServer.ConfigurationOptions
{
    public class AppSettings : Services.Identity.ConfigurationOptions.AppSettings
    {
        public IdentityServerOptions IdentityServer { get; set; }

        public Dictionary<string, string> SecurityHeaders { get; set; }

        public ExternalLoginOptions ExternalLogin { get; set; }

        public CookiePolicyOptions CookiePolicyOptions { get; set; }
    }
}
