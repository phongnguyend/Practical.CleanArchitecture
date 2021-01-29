using ClassifiedAds.Domain.Events;
using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using ClassifiedAds.Modules.Notification.Contracts.DTOs;
using ClassifiedAds.Modules.Notification.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Modules.Notification.EventHandlers
{
    public class EmailMessageCreatedEventHandler : IDomainEventHandler<EntityCreatedEvent<EmailMessage>>
    {
        private readonly IMessageSender<EmailMessageCreatedEvent> _emailMessageCreatedEventSender;

        public EmailMessageCreatedEventHandler(IMessageSender<EmailMessageCreatedEvent> emailMessageCreatedEventSender)
        {
            _emailMessageCreatedEventSender = emailMessageCreatedEventSender;
        }

        public async Task HandleAsync(EntityCreatedEvent<EmailMessage> domainEvent, CancellationToken cancellationToken = default)
        {
            // Handle the event here and we can also forward to external systems
            await _emailMessageCreatedEventSender.SendAsync(new EmailMessageCreatedEvent { Id = domainEvent.Entity.Id });
        }
    }
}
