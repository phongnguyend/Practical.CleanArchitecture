using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace ClassifiedAds.Persistence.Repositories
{
    public class UserRepository : Repository<User, Guid>, IUserRepository
    {
        public UserRepository(AdsDbContext dbContext)
            : base(dbContext)
        {
        }

        public IQueryable<User> GetAllIncludeTokens()
        {
            return GetAll().Include(x => x.Tokens);
        }
    }
}
