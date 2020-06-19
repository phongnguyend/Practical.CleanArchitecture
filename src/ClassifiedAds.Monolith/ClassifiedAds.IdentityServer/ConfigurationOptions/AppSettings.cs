using ClassifiedAds.Infrastructure.Logging;
using System.Collections.Generic;

namespace ClassifiedAds.IdentityServer.ConfigurationOptions
{
    public class AppSettings
    {
        public ConnectionStrings ConnectionStrings { get; set; }

        public LoggingOptions Logging { get; set; }

        public Dictionary<string, string> SecurityHeaders { get; set; }
    }
}
