using ClassifiedAds.Application;
using ClassifiedAds.Application.Decorators.AuditLog;
using ClassifiedAds.Application.Decorators.DatabaseRetry;
using ClassifiedAds.Services.Identity.Entities;
using ClassifiedAds.Services.Identity.Repositories;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Identity.Queries;

public class GetUsersQuery : IQuery<List<User>>
{
    public bool IncludeClaims { get; set; }
    public bool IncludeUserRoles { get; set; }
    public bool IncludeRoles { get; set; }
    public bool AsNoTracking { get; set; }
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
