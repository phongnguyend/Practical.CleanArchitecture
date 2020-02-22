using ClassifiedAds.DomainServices.DomainEvents;
using ClassifiedAds.DomainServices.Entities;

namespace ClassifiedAds.ApplicationServices.Events
{
    public class FileEntryUpdatedEventHandler : IDomainEventHandler<EntityUpdatedEvent<FileEntry>>
    {
        public void Handle(EntityUpdatedEvent<FileEntry> domainEvent)
        {
            // Handle the event here and we can also forward to external systems
        }
    }
}
