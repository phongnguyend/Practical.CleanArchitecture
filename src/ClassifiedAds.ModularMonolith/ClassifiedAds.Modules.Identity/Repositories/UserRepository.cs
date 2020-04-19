using ClassifiedAds.Modules.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace ClassifiedAds.Modules.Identity.Repositories
{
    public class UserRepository : Repository<User, Guid>, IUserRepository
    {
        public UserRepository(IdentityDbContext dbContext)
            : base(dbContext)
        {
        }

        public IQueryable<User> Get(UserQueryOptions queryOptions)
        {
            var query = GetAll();
            if (queryOptions.IncludeTokens)
            {
                query = query.Include(x => x.Tokens);
            }

            if (queryOptions.IncludeClaims)
            {
                query = query.Include(x => x.Claims);
            }

            if (queryOptions.IncludeUserRoles)
            {
                query = query.Include(x => x.UserRoles);
            }

            if (queryOptions.IncludeRoles)
            {
                query = query.Include("UserRoles.Role");
            }

            if (queryOptions.AsNoTracking)
            {
                query = query = query.AsNoTracking();
            }

            return query;
        }
    }
}
