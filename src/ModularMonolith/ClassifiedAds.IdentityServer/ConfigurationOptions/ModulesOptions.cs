using ClassifiedAds.Modules.Auth.ConfigurationOptions;
using ClassifiedAds.Modules.Identity.ConfigurationOptions;
using ClassifiedAds.Modules.Notification.ConfigurationOptions;

namespace ClassifiedAds.IdentityServer.ConfigurationOptions
{
    public class ModulesOptions
    {
        public AuthModuleOptions Auth { get; set; }

        public IdentityModuleOptions Identity { get; set; }

        public NotificationModuleOptions Notification { get; set; }
    }
}
