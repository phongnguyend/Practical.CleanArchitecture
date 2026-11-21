using System.ComponentModel.DataAnnotations;

namespace ClassifiedAds.Mobile.Core.Models;

public sealed class ConfigurationEntry : Record
{
    public string Key { get; set; } = "";

    public string Value { get; set; } = "";

    public string Description { get; set; } = "";

    public bool IsSensitive { get; set; }

    public string UpdatedDateTime { get; set; } = "";

    public override string Title => Key;

    public override object Payload() => new
    {
        id = string.IsNullOrEmpty(Id) ? null : Id,
        Key,
        Value,
        Description,
        IsSensitive
    };
    public override string? Validate() => Required(Key, "Key") ?? Required(Value, "Value");
}
