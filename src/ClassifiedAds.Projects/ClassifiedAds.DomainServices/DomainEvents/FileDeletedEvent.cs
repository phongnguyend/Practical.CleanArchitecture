using ClassifiedAds.DomainServices.Entities;

namespace ClassifiedAds.DomainServices.DomainEvents
{
    public class FileDeletedEvent
    {
        public FileEntry FileEntry { get; set; }
    }
}
