using ClassifiedAds.Domain.Entities;

namespace ClassifiedAds.Application.Events
{
    public class FileDeletedEvent
    {
        public FileEntry FileEntry { get; set; }
    }
}
