using System;

namespace ClassifiedAds.DomainServices.Entities
{
    public class Role : Entity<Guid>
    {
        public virtual string Name { get; set; }

        public virtual string NormalizedName { get; set; }

        public virtual string ConcurrencyStamp { get; set; }
    }
}
