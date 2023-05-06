using ClassifiedAds.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace ClassifiedAds.WebAPI.Models.Roles;

public static class RoleModelMappingConfiguration
{
    public static IEnumerable<RoleModel> ToModels(this IEnumerable<Role> entities)
    {
        return entities.Select(x => x.ToModel());
    }

    public static RoleModel ToModel(this Role entity)
    {
        if (entity == null)
        {
            return null;
        }

        return new RoleModel
        {
            Id = entity.Id,
            Name = entity.Name,
            NormalizedName = entity.NormalizedName,
            ConcurrencyStamp = entity.ConcurrencyStamp,
        };
    }
}
