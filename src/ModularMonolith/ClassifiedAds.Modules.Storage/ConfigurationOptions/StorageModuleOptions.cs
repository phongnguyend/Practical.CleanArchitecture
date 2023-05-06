using ClassifiedAds.Infrastructure.Storages;

namespace ClassifiedAds.Modules.Storage.ConfigurationOptions;

public class StorageModuleOptions : StorageOptions
{
    public ConnectionStringsOptions ConnectionStrings { get; set; }
}
