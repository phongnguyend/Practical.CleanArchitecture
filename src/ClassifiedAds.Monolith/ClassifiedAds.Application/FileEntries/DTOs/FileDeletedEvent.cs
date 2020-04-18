using ClassifiedAds.Domain.Entities;

namespace ClassifiedAds.Application.FileEntries.DTOs
{
    public class FileDeletedEvent
    {
        public FileEntry FileEntry { get; set; }
    }
}
