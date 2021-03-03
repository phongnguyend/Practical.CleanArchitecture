using ClassifiedAds.Infrastructure.MessageBrokers;
using ClassifiedAds.Infrastructure.Storages;

namespace ClassifiedAds.Modules.Storage.ConfigurationOptions
{
    public class StorageModuleOptions
    {
        public ConnectionStringsOptions ConnectionStrings { get; set; }

        public MessageBrokerOptions MessageBroker { get; set; }

        public StorageOptions Storage { get; set; }
    }
}
