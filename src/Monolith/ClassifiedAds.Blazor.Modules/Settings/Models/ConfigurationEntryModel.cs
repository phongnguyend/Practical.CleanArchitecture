using System;
using System.ComponentModel.DataAnnotations;

namespace ClassifiedAds.Blazor.Modules.Settings.Models;

public class ConfigurationEntryModel
{
    public Guid Id { get; set; }

    [Required]
    public string Key { get; set; }

    [Required]
    public string Value { get; set; }

    public string Description { get; set; }

    public bool IsSensitive { get; set; }

    public byte[] RowVersion { get; set; }

    public DateTimeOffset CreatedDateTime { get; set; }

    public DateTimeOffset? UpdatedDateTime { get; set; }
}
