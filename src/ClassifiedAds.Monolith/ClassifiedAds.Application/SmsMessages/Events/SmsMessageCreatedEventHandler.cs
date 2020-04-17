using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Events;

namespace ClassifiedAds.Application.SmsMessages.Events
{
    public class SmsMessageCreatedEventHandler : IDomainEventHandler<EntityCreatedEvent<SmsMessage>>
    {
        public SmsMessageCreatedEventHandler()
        {
        }

        public void Handle(EntityCreatedEvent<SmsMessage> domainEvent)
        {
            // Handle the event here and we can also forward to external systems
        }
    }
}
