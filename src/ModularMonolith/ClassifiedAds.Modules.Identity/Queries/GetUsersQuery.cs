using ClassifiedAds.Application;
using ClassifiedAds.Application.Decorators.AuditLog;
using ClassifiedAds.Application.Decorators.DatabaseRetry;
using ClassifiedAds.Contracts.Identity.DTOs;
using ClassifiedAds.Modules.Identity.Entities;
using ClassifiedAds.Modules.Identity.Repositories;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Modules.Identity.Queries.Roles;

public class GetUsersQuery : UserQueryOptions, IQuery<List<User>>
{
}

[AuditLog]
[DatabaseRetry(retryTimes: 4)]
public class GetUsersQueryHandler : IQueryHandler<GetUsersQuery, List<User>>
{
    private readonly IUserRepository _userRepository;

    public GetUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public Task<List<User>> HandleAsync(GetUsersQuery query, CancellationToken cancellationToken = default)
    {
        var db = _userRepository.Get(new UserQueryOptions
        {
            IncludeClaims = query.IncludeClaims,
            IncludeUserRoles = query.IncludeUserRoles,
            IncludeRoles = query.IncludeRoles,
            AsNoTracking = query.AsNoTracking,
        });

        return _userRepository.ToListAsync(db);
    }

}
