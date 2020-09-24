using AutoMapper;
using ClassifiedAds.Services.Identity.Entities;

namespace ClassifiedAds.Services.Identity.DTOs
{
    public class UserDTOMappingConfiguration : Profile
    {
        public UserDTOMappingConfiguration()
        {
            CreateMap<User, UserDTO>();
        }

    }
}
