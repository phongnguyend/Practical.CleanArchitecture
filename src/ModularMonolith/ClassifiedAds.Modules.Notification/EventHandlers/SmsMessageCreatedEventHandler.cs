using ClassifiedAds.Domain.Events;
using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using ClassifiedAds.Modules.Notification.Contracts.DTOs;
using ClassifiedAds.Modules.Notification.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Modules.Notification.EventHandlers
{
    public class SmsMessageCreatedEventHandler : IDomainEventHandler<EntityCreatedEvent<SmsMessage>>
    {
        private readonly IMessageSender<SmsMessageCreatedEvent> _smsMessageCreatedEventSender;

        public SmsMessageCreatedEventHandler(IMessageSender<SmsMessageCreatedEvent> smsMessageCreatedEventSender)
        {
            _smsMessageCreatedEventSender = smsMessageCreatedEventSender;
        }

        public async Task HandleAsync(EntityCreatedEvent<SmsMessage> domainEvent, CancellationToken cancellationToken = default)
        {
            // Handle the event here and we can also forward to external systems
           await _smsMessageCreatedEventSender.SendAsync(new SmsMessageCreatedEvent { Id = domainEvent.Entity.Id });
        }
    }
}
