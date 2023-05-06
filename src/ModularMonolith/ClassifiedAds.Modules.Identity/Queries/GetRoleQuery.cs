using ClassifiedAds.Application;
using ClassifiedAds.Modules.Identity.Entities;
using ClassifiedAds.Modules.Identity.Repositories;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Modules.Identity.Queries.Roles;

public class GetRoleQuery : IQuery<Role>
{
    public Guid Id { get; set; }
    public bool IncludeClaims { get; set; }
    public bool IncludeUserRoles { get; set; }
    public bool IncludeUsers { get; set; }
    public bool AsNoTracking { get; set; }
}

public class GetRoleQueryHandler : IQueryHandler<GetRoleQuery, Role>
{
    private readonly IRoleRepository _roleRepository;

    public GetRoleQueryHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public Task<Role> HandleAsync(GetRoleQuery query, CancellationToken cancellationToken = default)
    {
        var db = _roleRepository.Get(new RoleQueryOptions
        {
            IncludeClaims = query.IncludeClaims,
            IncludeUserRoles = query.IncludeUserRoles,
            IncludeUsers = query.IncludeUsers,
            AsNoTracking = query.AsNoTracking,
        });

        return _roleRepository.FirstOrDefaultAsync(db.Where(x => x.Id == query.Id));
    }
}
