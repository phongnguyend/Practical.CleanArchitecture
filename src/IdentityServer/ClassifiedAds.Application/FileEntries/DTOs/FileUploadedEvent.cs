using ClassifiedAds.Domain.Entities;

namespace ClassifiedAds.Application.FileEntries.DTOs
{
    public class FileUploadedEvent
    {
        public FileEntry FileEntry { get; set; }
    }
}
