using ClassifiedAds.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace ClassifiedAds.WebAPI.Models.ConfigurationEntries;

public static class ConfigurationEntryModelMappingConfiguration
{
    public static IEnumerable<ConfigurationEntryModel> ToModels(this IEnumerable<ConfigurationEntry> entities)
    {
        return entities.Select(x => x.ToModel());
    }

    public static ConfigurationEntryModel ToModel(this ConfigurationEntry entity)
    {
        if (entity == null)
        {
            return null;
        }

        return new ConfigurationEntryModel
        {
            Id = entity.Id,
            Key = entity.Key,
            Value = entity.Value,
            Description = entity.Description,
            IsSensitive = entity.IsSensitive,
            CreatedDateTime = entity.CreatedDateTime,
            UpdatedDateTime = entity.UpdatedDateTime,
        };
    }

    public static ConfigurationEntry ToEntity(this ConfigurationEntryModel model)
    {
        return new ConfigurationEntry
        {
            Id = model.Id,
            Key = model.Key,
            Value = model.Value,
            Description = model.Description,
            IsSensitive = model.IsSensitive,
            CreatedDateTime = model.CreatedDateTime,
            UpdatedDateTime = model.UpdatedDateTime,
        };
    }
}
