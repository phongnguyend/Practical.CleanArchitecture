using ClassifiedAds.Domain.Events;
using ClassifiedAds.Domain.Entities;

namespace ClassifiedAds.Application.Events
{
    public class FileEntryUpdatedEventHandler : IDomainEventHandler<EntityUpdatedEvent<FileEntry>>
    {
        public void Handle(EntityUpdatedEvent<FileEntry> domainEvent)
        {
            // Handle the event here and we can also forward to external systems
        }
    }
}
