using ClassifiedAds.DomainServices.Entities;

namespace ClassifiedAds.ApplicationServices.Events
{
    public class FileUploadedEvent
    {
        public FileEntry FileEntry { get; set; }
    }
}
