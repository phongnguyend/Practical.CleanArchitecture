using System;

namespace ClassifiedAds.Domain.Entities
{
    public class Role : Entity<Guid>
    {
        public virtual string Name { get; set; }

        public virtual string NormalizedName { get; set; }

        public virtual string ConcurrencyStamp { get; set; }
    }
}
