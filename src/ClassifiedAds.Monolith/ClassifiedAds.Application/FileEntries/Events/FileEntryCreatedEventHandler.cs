using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Events;
using ClassifiedAds.Domain.Infrastructure.MessageBrokers;

namespace ClassifiedAds.Application.FileEntries.Events
{
    public class FileEntryCreatedEventHandler : IDomainEventHandler<EntityCreatedEvent<FileEntry>>
    {
        private readonly IMessageSender<FileUploadedEvent> _fileUploadedEventSender;

        public FileEntryCreatedEventHandler(IMessageSender<FileUploadedEvent> fileUploadedEventSender)
        {
            _fileUploadedEventSender = fileUploadedEventSender;
        }

        public void Handle(EntityCreatedEvent<FileEntry> domainEvent)
        {
            // Handle the event here and we can also forward to external systems
            _fileUploadedEventSender.Send(new FileUploadedEvent
            {
                FileEntry = domainEvent.Entity,
            });
        }
    }
}
