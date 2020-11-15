using ClassifiedAds.Modules.Identity.Entities;
using System.Collections.Generic;
using System.Linq;

namespace ClassifiedAds.Modules.Identity.DTOs.Roles
{
    public static class RoleDTOMappingConfiguration
    {
        public static IEnumerable<RoleDTO> ToDTOs(this IEnumerable<Role> entities)
        {
            return entities.Select(x => x.ToDTO());
        }

        public static RoleDTO ToDTO(this Role entity)
        {
            return new RoleDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                NormalizedName = entity.NormalizedName,
                ConcurrencyStamp = entity.ConcurrencyStamp,
            };
        }
    }
}
