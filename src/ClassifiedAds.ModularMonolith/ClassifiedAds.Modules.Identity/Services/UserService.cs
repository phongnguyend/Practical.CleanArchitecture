using ClassifiedAds.Application;
using ClassifiedAds.Modules.Identity.Contracts.DTOs;
using ClassifiedAds.Modules.Identity.Contracts.Services;
using ClassifiedAds.Modules.Identity.Queries.Roles;
using System.Collections.Generic;
using System.Linq;

namespace ClassifiedAds.Modules.Identity.Services
{
    public class UserService : IUserService
    {
        private readonly Dispatcher _dispatcher;

        public UserService(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public List<UserDTO> GetUsers(UserQueryOptions query)
        {
            var users = _dispatcher.Dispatch(new GetUsersQuery
            {
                IncludeClaims = query.IncludeClaims,
                IncludeUserRoles = query.IncludeUserRoles,
                IncludeRoles = query.IncludeRoles,
                AsNoTracking = query.AsNoTracking,
            }).Select(x => new UserDTO
            {
                Id = x.Id,
                UserName = x.UserName,
            })
            .ToList();

            return users;
        }
    }
}
