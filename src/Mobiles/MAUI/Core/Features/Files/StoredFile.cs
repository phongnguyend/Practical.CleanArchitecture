using System.ComponentModel.DataAnnotations;

namespace ClassifiedAds.Mobile.Core.Models;

public sealed class StoredFile : Record
{
    public string Name { get; set; } = "";

    public string Description { get; set; } = "";

    public bool Encrypted { get; set; }

    public string FileName { get; set; } = "";

    public long Size { get; set; }

    public string UploadedTime { get; set; } = "";

    public string FileLocation { get; set; } = "";

    public override string Title => Name;

    public override object Payload() => new
    {
        Id,
        Name,
        Description,
        Encrypted,
        FileName
    };
    public override string? Validate() => Required(Name, "Name", 3) ?? Required(Description, "Description") ?? (Description.Length > 100 ? "Description must be at most 100 characters." : null);
}
