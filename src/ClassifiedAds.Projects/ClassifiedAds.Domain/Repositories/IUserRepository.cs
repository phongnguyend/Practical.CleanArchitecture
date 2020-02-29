using ClassifiedAds.Domain.Entities;
using System;
using System.Linq;

namespace ClassifiedAds.Domain.Repositories
{
    public interface IUserRepository : IRepository<User, Guid>
    {
        IQueryable<User> GetAllIncludeTokens();
    }
}
