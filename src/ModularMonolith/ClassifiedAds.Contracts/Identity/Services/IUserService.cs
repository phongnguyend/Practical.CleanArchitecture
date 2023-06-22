using ClassifiedAds.Contracts.Identity.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClassifiedAds.Contracts.Identity.Services;

public interface IUserService
{
    Task<List<UserDTO>> GetUsersAsync(UserQueryOptions query);
}
