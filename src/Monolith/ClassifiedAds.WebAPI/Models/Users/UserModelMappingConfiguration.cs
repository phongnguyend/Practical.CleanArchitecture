using AutoMapper;
using ClassifiedAds.Domain.Entities;

namespace ClassifiedAds.WebAPI.Models.Users
{
    public class UserModelMappingConfiguration : Profile
    {
        public UserModelMappingConfiguration()
        {
            CreateMap<User, UserModel>();
        }

    }
}
