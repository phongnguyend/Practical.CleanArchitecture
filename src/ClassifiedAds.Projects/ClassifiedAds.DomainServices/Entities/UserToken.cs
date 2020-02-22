using System;

namespace ClassifiedAds.DomainServices.Entities
{
    public class UserToken : AggregateRoot<Guid>
    {
        public Guid UserId { get; set; }

        public string LoginProvider { get; set; }

        public string TokenName { get; set; }

        public string TokenValue { get; set; }
    }
}
