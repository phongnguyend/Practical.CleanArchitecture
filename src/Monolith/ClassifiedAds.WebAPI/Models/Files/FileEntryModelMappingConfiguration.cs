using ClassifiedAds.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace ClassifiedAds.WebAPI.Models.Files;

public static class FileEntryModelMappingConfiguration
{
    public static IEnumerable<FileEntryModel> ToModels(this IEnumerable<FileEntry> entities)
    {
        return entities.Select(x => x.ToModel());
    }

    public static FileEntryModel ToModel(this FileEntry entity)
    {
        if (entity == null)
        {
            return null;
        }

        return new FileEntryModel
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Size = entity.Size,
            UploadedTime = entity.UploadedTime,
            FileName = entity.FileName,
            FileLocation = entity.FileLocation,
            Encrypted = entity.Encrypted,
        };
    }
}
