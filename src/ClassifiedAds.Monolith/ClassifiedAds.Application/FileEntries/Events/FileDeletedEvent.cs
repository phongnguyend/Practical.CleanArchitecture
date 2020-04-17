using ClassifiedAds.Domain.Entities;

namespace ClassifiedAds.Application.FileEntries.Events
{
    public class FileDeletedEvent
    {
        public FileEntry FileEntry { get; set; }
    }
}
