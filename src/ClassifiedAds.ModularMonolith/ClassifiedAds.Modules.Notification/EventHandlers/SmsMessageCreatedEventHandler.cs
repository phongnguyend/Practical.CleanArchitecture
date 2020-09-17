using ClassifiedAds.Domain.Events;
using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using ClassifiedAds.Modules.Notification.Contracts.DTOs;
using ClassifiedAds.Modules.Notification.Entities;

namespace ClassifiedAds.Modules.Notification.EventHandlers
{
    public class SmsMessageCreatedEventHandler : IDomainEventHandler<EntityCreatedEvent<SmsMessage>>
    {
        private readonly IMessageSender<SmsMessageCreatedEvent> _smsMessageCreatedEventSender;

        public SmsMessageCreatedEventHandler(IMessageSender<SmsMessageCreatedEvent> smsMessageCreatedEventSender)
        {
            _smsMessageCreatedEventSender = smsMessageCreatedEventSender;
        }

        public void Handle(EntityCreatedEvent<SmsMessage> domainEvent)
        {
            // Handle the event here and we can also forward to external systems
            _smsMessageCreatedEventSender.Send(new SmsMessageCreatedEvent { Id = domainEvent.Entity.Id });
        }
    }
}
