using System.ComponentModel.DataAnnotations;

namespace ClassifiedAds.Mobile.Core.Models;

public sealed class Product : Record
{
    public string Name { get; set; } = "";

    public string Code { get; set; } = "";

    public string Description { get; set; } = "";

    public decimal Price { get; set; }

    public double StarRating { get; set; }

    public string ImageUrl { get; set; } = "";

    public override string Title => Name;

    public override object Payload() => new
    {
        id = string.IsNullOrEmpty(Id) ? null : Id,
        Name,
        Code,
        Description
    };
    public override string? Validate() => Required(Name, "Name", 3) ?? Required(Code, "Code") ?? (Code.Length > 10 ? "Code must be at most 10 characters." : null) ?? Required(Description, "Description") ?? (Description.Length > 100 ? "Description must be at most 100 characters." : null);
}
