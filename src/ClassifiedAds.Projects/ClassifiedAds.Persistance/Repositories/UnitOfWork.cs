using ClassifiedAds.DomainServices.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace ClassifiedAds.Persistance.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AdsDbContext _dbContext;
        private IDbContextTransaction _dbContextTransaction;

        public UnitOfWork(AdsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void BeginTransaction()
        {
            _dbContextTransaction = _dbContext.Database.BeginTransaction();
        }

        public void CommitTransaction()
        {
            _dbContextTransaction.Commit();
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }
    }
}
