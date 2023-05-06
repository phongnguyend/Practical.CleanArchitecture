using ClassifiedAds.Modules.AuditLog.ConfigurationOptions;
using ClassifiedAds.Modules.Configuration.ConfigurationOptions;
using ClassifiedAds.Modules.Identity.ConfigurationOptions;
using ClassifiedAds.Modules.Notification.ConfigurationOptions;
using ClassifiedAds.Modules.Product.ConfigurationOptions;
using ClassifiedAds.Modules.Storage.ConfigurationOptions;

namespace ClassifiedAds.WebAPI.ConfigurationOptions;

public class ModulesOptions
{
    public AuditLogModuleOptions AuditLog { get; set; }

    public ConfigurationModuleOptions Configuration { get; set; }

    public IdentityModuleOptions Identity { get; set; }

    public NotificationModuleOptions Notification { get; set; }

    public ProductModuleOptions Product { get; set; }

    public StorageModuleOptions Storage { get; set; }
}
