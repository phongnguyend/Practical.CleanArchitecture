using ClassifiedAds.Domain.Entities;
using System;

namespace ClassifiedAds.Modules.Notification.Entities
{
    public class SmsMessage : AggregateRoot<Guid>
    {
        public string Message { get; set; }

        public string PhoneNumber { get; set; }
    }
}
