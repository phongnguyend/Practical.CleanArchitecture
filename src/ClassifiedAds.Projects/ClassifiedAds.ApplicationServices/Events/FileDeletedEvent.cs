using ClassifiedAds.DomainServices.Entities;

namespace ClassifiedAds.ApplicationServices.Events
{
    public class FileDeletedEvent
    {
        public FileEntry FileEntry { get; set; }
    }
}
