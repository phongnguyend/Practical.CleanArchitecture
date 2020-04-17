namespace ClassifiedAds.BackgroundServer.ConfigurationOptions.MessageBroker
{
    public class KafkaOptions
    {
        public string BootstrapServers { get; set; }
        public string Topic_FileUploaded { get; set; }
        public string Topic_FileDeleted { get; set; }
        public string GroupId { get; set; }
    }
}
