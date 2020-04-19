using ClassifiedAds.Domain.Events;
using ClassifiedAds.Modules.Notification.Entities;

namespace ClassifiedAds.Modules.Notification.EventHandlers
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
