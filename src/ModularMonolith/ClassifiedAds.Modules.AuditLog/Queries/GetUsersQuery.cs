using ClassifiedAds.Application;
using ClassifiedAds.Modules.Identity.Contracts.DTOs;
using ClassifiedAds.Modules.Identity.Contracts.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Modules.AuditLog.Queries
{
    public class GetUsersQuery : UserQueryOptions, IQuery<List<UserDTO>>
    {
    }

    public class GetUsersQueryHandler : IQueryHandler<GetUsersQuery, List<UserDTO>>
    {
        private readonly IUserService _userService;

        public GetUsersQueryHandler(IUserService userService)
        {
            _userService = userService;
        }

        public Task<List<UserDTO>> HandleAsync(GetUsersQuery query, CancellationToken cancellationToken = default)
        {
            return _userService.GetUsersAsync(query);
        }
    }
}
