using ClassifiedAds.Modules.Identity.Contracts.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClassifiedAds.Modules.Identity.Contracts.Services
{
    public interface IUserService
    {
        Task<List<UserDTO>> GetUsersAsync(UserQueryOptions query);
    }
}
