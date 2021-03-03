using System;

namespace ClassifiedAds.Domain.Entities
{
    public class ConfigurationEntry : AggregateRoot<Guid>
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public string Description { get; set; }

        public bool IsSensitive { get; set; }
    }
}
