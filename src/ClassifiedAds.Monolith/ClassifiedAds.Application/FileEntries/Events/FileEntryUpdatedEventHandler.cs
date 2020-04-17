using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Events;

namespace ClassifiedAds.Application.FileEntries.Events
{
    public class FileEntryUpdatedEventHandler : IDomainEventHandler<EntityUpdatedEvent<FileEntry>>
    {
        public void Handle(EntityUpdatedEvent<FileEntry> domainEvent)
        {
            // Handle the event here and we can also forward to external systems
        }
    }
}
