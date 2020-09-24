using AutoMapper;
using ClassifiedAds.Services.Identity.Entities;

namespace ClassifiedAds.Services.Identity.DTOs
{
    public class RoleDTOMappingConfiguration : Profile
    {
        public RoleDTOMappingConfiguration()
        {
            CreateMap<Role, RoleDTO>();
        }
    }
}
