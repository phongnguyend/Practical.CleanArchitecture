using ClassifiedAds.Application.EmailMessages.DTOs;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Events;
using ClassifiedAds.Domain.Infrastructure.MessageBrokers;

namespace ClassifiedAds.Application.EmailMessages.Events
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
