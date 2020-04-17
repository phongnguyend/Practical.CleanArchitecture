using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Events;

namespace ClassifiedAds.Application.EmailMessages.Events
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
