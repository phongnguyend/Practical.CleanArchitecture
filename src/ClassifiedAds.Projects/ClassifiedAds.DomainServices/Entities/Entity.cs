using System.ComponentModel.DataAnnotations;

namespace ClassifiedAds.DomainServices.Entities
{
    public abstract class Entity<TKey> : IHasKey<TKey>, ITrackable
    {
        public TKey Id { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
