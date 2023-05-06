using ClassifiedAds.Services.Storage.Entities;

namespace ClassifiedAds.Services.Storage.DTOs;

public class FileUploadedEvent
{
    public FileEntry FileEntry { get; set; }
}
