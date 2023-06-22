using ClassifiedAds.Application;
using ClassifiedAds.Contracts.Identity.DTOs;
using ClassifiedAds.Contracts.Identity.Services;
using ClassifiedAds.Modules.Identity.Queries.Roles;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassifiedAds.Modules.Identity.Services;

public class UserService : IUserService
{
    private readonly Dispatcher _dispatcher;

    public UserService(Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public async Task<List<UserDTO>> GetUsersAsync(UserQueryOptions query)
    {
        var users = (await _dispatcher.DispatchAsync(new GetUsersQuery
        {
            IncludeClaims = query.IncludeClaims,
            IncludeUserRoles = query.IncludeUserRoles,
            IncludeRoles = query.IncludeRoles,
            AsNoTracking = query.AsNoTracking,
        })).Select(x => new UserDTO
        {
            Id = x.Id,
            UserName = x.UserName,
        })
        .ToList();

        return users;
    }
}
