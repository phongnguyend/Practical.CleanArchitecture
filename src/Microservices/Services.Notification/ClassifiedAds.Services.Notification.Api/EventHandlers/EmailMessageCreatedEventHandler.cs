using ClassifiedAds.Domain.Events;
using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using ClassifiedAds.Services.Notification.DTOs;
using ClassifiedAds.Services.Notification.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Notification.EventHandlers
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
