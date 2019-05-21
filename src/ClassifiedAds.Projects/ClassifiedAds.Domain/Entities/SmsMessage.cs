using System;

namespace ClassifiedAds.Domain.Entities
{
    public class SmsMessage : Entity<Guid>
    {
        public string Message { get; set; }

        public string PhoneNumber { get; set; }
    }
}
