using AutoMapper;
using ClassifiedAds.Modules.Identity.Entities;

namespace ClassifiedAds.Modules.Identity.DTOs.Roles
{
    public class RoleDTOMappingConfiguration : Profile
    {
        public RoleDTOMappingConfiguration()
        {
            CreateMap<Role, RoleDTO>();
        }
    }
}
