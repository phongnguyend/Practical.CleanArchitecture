using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Services.Identity.Entities;
using System;
using System.Linq;

namespace ClassifiedAds.Services.Identity.Repositories;

public class UserQueryOptions
{
    public bool IncludeClaims { get; set; }
    public bool IncludeUserRoles { get; set; }
    public bool IncludeRoles { get; set; }
    public bool IncludeTokens { get; set; }
    public bool AsNoTracking { get; set; }
}

public interface IUserRepository : IRepository<User, Guid>
{
    IQueryable<User> Get(UserQueryOptions queryOptions);
}
