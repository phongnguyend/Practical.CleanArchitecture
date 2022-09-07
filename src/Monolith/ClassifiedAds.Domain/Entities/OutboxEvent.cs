using System;

namespace ClassifiedAds.Domain.Entities
{
    public class OutboxEvent : AggregateRoot<long>
    {
        public string EventType { get; set; }

        public Guid TriggeredById { get; set; }

        public string ObjectId { get; set; }

        public string Message { get; set; }

        public bool Published { get; set; }
    }
}
