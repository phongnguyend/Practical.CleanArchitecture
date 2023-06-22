using ClassifiedAds.Contracts.Identity.DTOs;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Modules.Identity.Entities;
using System;
using System.Linq;

namespace ClassifiedAds.Modules.Identity.Repositories;

public interface IUserRepository : IRepository<User, Guid>
{
    IQueryable<User> Get(UserQueryOptions queryOptions);
}
