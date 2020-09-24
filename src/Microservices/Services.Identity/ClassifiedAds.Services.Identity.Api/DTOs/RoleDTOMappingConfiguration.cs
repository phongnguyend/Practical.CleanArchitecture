using AutoMapper;
using ClassifiedAds.Services.Identity.Entities;

namespace ClassifiedAds.Services.Identity.DTOs.Roles
{
    public class RoleDTOMappingConfiguration : Profile
    {
        public RoleDTOMappingConfiguration()
        {
            CreateMap<Role, RoleDTO>();
        }
    }
}
