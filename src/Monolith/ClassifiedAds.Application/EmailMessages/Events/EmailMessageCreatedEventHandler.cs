using ClassifiedAds.Application.EmailMessages.DTOs;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Events;
using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using System.Threading.Tasks;

namespace ClassifiedAds.Application.EmailMessages.Events
{
    public class EmailMessageCreatedEventHandler : IDomainEventHandler<EntityCreatedEvent<EmailMessage>>
    {
        private readonly IMessageSender<EmailMessageCreatedEvent> _emailMessageCreatedEventSender;

        public EmailMessageCreatedEventHandler(IMessageSender<EmailMessageCreatedEvent> emailMessageCreatedEventSender)
        {
            _emailMessageCreatedEventSender = emailMessageCreatedEventSender;
        }

        public async Task HandleAsync(EntityCreatedEvent<EmailMessage> domainEvent)
        {
            // Handle the event here and we can also forward to external systems
            await _emailMessageCreatedEventSender.SendAsync(new EmailMessageCreatedEvent { Id = domainEvent.Entity.Id });
        }
    }
}
