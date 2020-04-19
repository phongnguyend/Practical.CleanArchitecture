using ClassifiedAds.Modules.Storage.Entities;

namespace ClassifiedAds.Modules.Storage.DTOs
{
    public class FileDeletedEvent
    {
        public FileEntry FileEntry { get; set; }
    }
}
