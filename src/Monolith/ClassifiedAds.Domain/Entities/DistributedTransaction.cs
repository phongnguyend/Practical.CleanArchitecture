using System;
using System.Collections.Generic;

namespace ClassifiedAds.Domain.Entities
{
    public class DistributedTransaction : AggregateRoot<Guid>
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public string Payload { get; set; }

        public string Status { get; set; }

        public bool Completed { get; set; }

        public ICollection<DistributedTransactionProperty> DistributedTransactionProperties { get; set; }

        public ICollection<DistributedTransactionDetail> DistributedTransactionDetails { get; set; }
    }
}
