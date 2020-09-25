using AutoMapper;
using ClassifiedAds.Domain.Entities;

namespace ClassifiedAds.WebAPI.Models.Roles
{
    public class RoleModelMappingConfiguration : Profile
    {
        public RoleModelMappingConfiguration()
        {
            CreateMap<Role, RoleModel>();
        }
    }
}
