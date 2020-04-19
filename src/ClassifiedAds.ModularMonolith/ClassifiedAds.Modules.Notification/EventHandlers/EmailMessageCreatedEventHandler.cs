using ClassifiedAds.Domain.Events;
using ClassifiedAds.Modules.Notification.Entities;

namespace ClassifiedAds.Modules.Notification.EventHandlers
{
    public class EmailMessageCreatedEventHandler : IDomainEventHandler<EntityCreatedEvent<EmailMessage>>
    {
        public EmailMessageCreatedEventHandler()
        {
        }

        public void Handle(EntityCreatedEvent<EmailMessage> domainEvent)
        {
            // Handle the event here and we can also forward to external systems
        }
    }
}
