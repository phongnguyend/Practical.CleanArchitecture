using ClassifiedAds.Domain.Events;
using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using ClassifiedAds.Modules.Notification.Contracts.DTOs;
using ClassifiedAds.Modules.Notification.Entities;

namespace ClassifiedAds.Modules.Notification.EventHandlers
{
    public class EmailMessageCreatedEventHandler : IDomainEventHandler<EntityCreatedEvent<EmailMessage>>
    {
        private readonly IMessageSender<EmailMessageCreatedEvent> _emailMessageCreatedEventSender;

        public EmailMessageCreatedEventHandler(IMessageSender<EmailMessageCreatedEvent> emailMessageCreatedEventSender)
        {
            _emailMessageCreatedEventSender = emailMessageCreatedEventSender;
        }

        public void Handle(EntityCreatedEvent<EmailMessage> domainEvent)
        {
            // Handle the event here and we can also forward to external systems
            _emailMessageCreatedEventSender.Send(new EmailMessageCreatedEvent { Id = domainEvent.Entity.Id });
        }
    }
}
