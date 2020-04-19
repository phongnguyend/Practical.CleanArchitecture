using ClassifiedAds.Domain.Entities;
using System;

namespace ClassifiedAds.Modules.Notification.Entities
{
    public class EmailMessage : AggregateRoot<Guid>
    {
        public string From { get; set; }

        public string To { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public DateTime? SentDate { get; set; }
    }
}
