using System;

namespace ClassifiedAds.DomainServices.Entities
{
    public class ConfigurationEntry : Entity<Guid>
    {
        public string Key { get; set; }

        public string Value { get; set; }
    }
}
