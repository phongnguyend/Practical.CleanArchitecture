using ClassifiedAds.DomainServices.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClassifiedAds.Persistance.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AdsDbContext _dbContext;

        public UnitOfWork(AdsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }
    }
}
