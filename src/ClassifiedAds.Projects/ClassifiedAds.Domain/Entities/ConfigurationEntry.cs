using System;

namespace ClassifiedAds.Domain.Entities
{
    public class ConfigurationEntry : Entity<Guid>
    {
        public string Key { get; set; }

        public string Value { get; set; }
    }
}
