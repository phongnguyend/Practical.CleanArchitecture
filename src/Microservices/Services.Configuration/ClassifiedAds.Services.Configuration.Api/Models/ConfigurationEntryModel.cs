using System;

namespace ClassifiedAds.Services.Configuration.Models;

public class ConfigurationEntryModel
{
    public Guid Id { get; set; }

    public string Key { get; set; }

    public string Value { get; set; }

    public string Description { get; set; }

    public bool IsSensitive { get; set; }

    public byte[] RowVersion { get; set; }

    public DateTimeOffset CreatedDateTime { get; set; }

    public DateTimeOffset? UpdatedDateTime { get; set; }
}
