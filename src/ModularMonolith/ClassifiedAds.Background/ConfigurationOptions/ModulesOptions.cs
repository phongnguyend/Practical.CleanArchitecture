using ClassifiedAds.Modules.Notification.ConfigurationOptions;
using ClassifiedAds.Modules.Storage.ConfigurationOptions;

namespace ClassifiedAds.Background.ConfigurationOptions;

public class ModulesOptions
{
    public NotificationModuleOptions Notification { get; set; }

    public StorageModuleOptions Storage { get; set; }
}
