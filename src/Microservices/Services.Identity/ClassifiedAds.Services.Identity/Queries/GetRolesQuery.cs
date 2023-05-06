using ClassifiedAds.Application;
using ClassifiedAds.Application.Decorators.DatabaseRetry;
using ClassifiedAds.Services.Identity.Entities;
using ClassifiedAds.Services.Identity.Repositories;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Identity.Queries;

public class GetRolesQuery : IQuery<List<Role>>
{
    public bool IncludeClaims { get; set; }
    public bool IncludeUserRoles { get; set; }
    public bool AsNoTracking { get; set; }
}

[DatabaseRetry(retryTimes: 2)]
public class GetRolesQueryHandler : IQueryHandler<GetRolesQuery, List<Role>>
{
    private readonly IRoleRepository _roleRepository;

    public GetRolesQueryHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public Task<List<Role>> HandleAsync(GetRolesQuery query, CancellationToken cancellationToken = default)
    {
        var db = _roleRepository.Get(new RoleQueryOptions
        {
            IncludeClaims = query.IncludeClaims,
            IncludeUserRoles = query.IncludeUserRoles,
            AsNoTracking = query.AsNoTracking,
        });

        return _roleRepository.ToListAsync(db);
    }
}
