using ClassifiedAds.Domain.Entities;

namespace ClassifiedAds.Application.FileEntries.Events
{
    public class FileUploadedEvent
    {
        public FileEntry FileEntry { get; set; }
    }
}
