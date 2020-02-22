using ClassifiedAds.DomainServices.DomainEvents;
using ClassifiedAds.DomainServices.Entities;
using ClassifiedAds.DomainServices.Infrastructure.MessageBrokers;

namespace ClassifiedAds.ApplicationServices.Events
{
    public class FileEntryDeletedEventHandler : IDomainEventHandler<EntityDeletedEvent<FileEntry>>
    {
        private readonly IMessageSender<FileDeletedEvent> _fileDeletedEventSender;

        public FileEntryDeletedEventHandler(IMessageSender<FileDeletedEvent> fileDeletedEventSender)
        {
            _fileDeletedEventSender = fileDeletedEventSender;
        }

        public void Handle(EntityDeletedEvent<FileEntry> domainEvent)
        {
            // Handle the event here and we can also forward to external systems
            _fileDeletedEventSender.Send(new FileDeletedEvent
            {
                FileEntry = domainEvent.Entity,
            });
        }
    }
}
