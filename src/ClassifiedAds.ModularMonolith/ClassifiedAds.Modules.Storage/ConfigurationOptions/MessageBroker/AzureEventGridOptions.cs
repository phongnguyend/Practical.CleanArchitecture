namespace ClassifiedAds.Modules.Storage.ConfigurationOptions.MessageBroker
{
    public class AzureEventGridOptions
    {
        public string DomainEndpoint { get; set; }

        public string DomainKey { get; set; }

        public string Topic_FileUploaded { get; set; }

        public string Topic_FileDeleted { get; set; }
    }
}
