using ClassifiedAds.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace ClassifiedAds.WebAPI.Models.Roles
{
    public static class RoleModelMappingConfiguration
    {
        public static IEnumerable<RoleModel> ToDTOs(this IEnumerable<Role> entities)
        {
            return entities.Select(x => x.ToDTO());
        }

        public static RoleModel ToDTO(this Role entity)
        {
            return new RoleModel
            {
                Id = entity.Id,
                Name = entity.Name,
                NormalizedName = entity.NormalizedName,
                ConcurrencyStamp = entity.ConcurrencyStamp,
            };
        }
    }
}
