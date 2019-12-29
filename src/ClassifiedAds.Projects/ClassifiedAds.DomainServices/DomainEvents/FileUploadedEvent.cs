using ClassifiedAds.DomainServices.Entities;

namespace ClassifiedAds.DomainServices.DomainEvents
{
    public class FileUploadedEvent
    {
        public FileEntry FileEntry { get; set; }
    }
}
