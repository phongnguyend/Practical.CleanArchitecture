using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Services.Identity.Entities;
using System;
using System.Linq;

namespace ClassifiedAds.Services.Identity.Repositories;

public class RoleQueryOptions
{
    public bool IncludeClaims { get; set; }
    public bool IncludeUserRoles { get; set; }
    public bool IncludeUsers { get; set; }
    public bool AsNoTracking { get; set; }
}

public interface IRoleRepository : IRepository<Role, Guid>
{
    IQueryable<Role> Get(RoleQueryOptions queryOptions);
}
