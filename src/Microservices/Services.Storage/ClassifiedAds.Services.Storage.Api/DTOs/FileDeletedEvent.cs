using ClassifiedAds.Services.Storage.Entities;

namespace ClassifiedAds.Services.Storage.DTOs;

public class FileDeletedEvent
{
    public FileEntry FileEntry { get; set; }
}
