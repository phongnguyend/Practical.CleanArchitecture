using ClassifiedAds.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ClassifiedAds.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AdsDbContext _dbContext;
        private IDbContextTransaction _dbContextTransaction;

        public UnitOfWork(AdsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            _dbContextTransaction = _dbContext.Database.BeginTransaction(isolationLevel);
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
