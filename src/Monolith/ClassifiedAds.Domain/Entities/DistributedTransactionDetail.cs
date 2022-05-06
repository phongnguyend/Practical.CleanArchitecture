using System;

namespace ClassifiedAds.Domain.Entities
{
    public class DistributedTransactionDetail : Entity<Guid>
    {
        public string Payload { get; set; }

        public string Result { get; set; }

        public string Status { get; set; }

        public Guid DistributedTransactionId { get; set; }
    }
}
