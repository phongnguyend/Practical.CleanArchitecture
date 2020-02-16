using ClassifiedAds.DomainServices.Entities;
using ClassifiedAds.DomainServices.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace ClassifiedAds.Persistence.Repositories
{
    public class StoreRepository : Repository<Store>, IStoreRepository
    {
        public StoreRepository(AdsDbContext dbContext)
            : base(dbContext)
        {
        }

        public Store GetStoreIncludeProducts(Guid Id)
        {
            return DbSet.Include(x => x.Products)
                // .ThenInclude(s => s.Voters)
                .FirstOrDefault(x => x.Id == Id);
        }
    }
}
