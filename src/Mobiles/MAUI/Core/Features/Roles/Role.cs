using System.ComponentModel.DataAnnotations;

namespace ClassifiedAds.Mobile.Core.Models;

public sealed class Role : Record
{
    public string Name { get; set; } = "";

    public string NormalizedName { get; set; } = "";

    public override string Title => Name;

    public override object Payload() => new
    {
        id = string.IsNullOrEmpty(Id) ? null : Id,
        Name
    };
    public override string? Validate() => Required(Name, "Name");
}
