using ClassifiedAds.Domain.Entities;

namespace ClassifiedAds.Application.Events
{
    public class FileUploadedEvent
    {
        public FileEntry FileEntry { get; set; }
    }
}
