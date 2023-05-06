using System;

namespace ClassifiedAds.WebAPI.Models.Files;

public class FileEntryModel
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public long Size { get; set; }

    public DateTimeOffset UploadedTime { get; set; }

    public string FileName { get; set; }

    public string FileLocation { get; set; }

    public bool Encrypted { get; set; }
}
