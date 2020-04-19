using AutoMapper;
using ClassifiedAds.Modules.Identity.Entities;

namespace ClassifiedAds.Modules.Identity.DTOs.Users
{
    public class UserDTOMappingConfiguration : Profile
    {
        public UserDTOMappingConfiguration()
        {
            CreateMap<User, UserDTO>();
        }

    }
}
