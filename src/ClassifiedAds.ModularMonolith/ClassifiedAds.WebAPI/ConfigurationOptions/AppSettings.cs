using ClassifiedAds.Infrastructure.Logging;
using ClassifiedAds.Modules.Storage;
using ClassifiedAds.Modules.Storage.ConfigurationOptions.MessageBroker;

namespace ClassifiedAds.WebAPI.ConfigurationOptions
{
    public class AppSettings
    {
        public ConnectionStrings ConnectionStrings { get; set; }

        public LoggerOptions LoggerOptions { get; set; }

        public IdentityServerAuthentication IdentityServerAuthentication { get; set; }

        public string AllowedHosts { get; set; }

        public CORS CORS { get; set; }

        public StorageOptions Storage { get; set; }

        public MessageBrokerOptions MessageBroker { get; set; }
    }
}
