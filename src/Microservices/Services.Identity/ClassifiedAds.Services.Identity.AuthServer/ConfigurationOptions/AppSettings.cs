﻿using ClassifiedAds.IdentityServer.ConfigurationOptions.ExternalLogin;
using System.Collections.Generic;

namespace ClassifiedAds.IdentityServer.ConfigurationOptions
{
    public class AppSettings: Services.Identity.ConfigurationOptions.AppSettings
    {
        public Dictionary<string, string> SecurityHeaders { get; set; }

        public CertificatesOptions Certificates { get; set; }

        public ExternalLoginOptions ExternalLogin { get; set; }

        public CookiePolicyOptions CookiePolicyOptions { get; set; }
    }
}
