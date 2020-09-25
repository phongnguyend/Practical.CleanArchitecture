using ClassifiedAds.Modules.Identity.Contracts.DTOs;
using System.Collections.Generic;

namespace ClassifiedAds.Modules.Identity.Contracts.Services
{
    public interface IUserService
    {
        List<UserDTO> GetUsers(UserQueryOptions query);
    }
}
