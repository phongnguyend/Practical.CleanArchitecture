using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ClassifiedAds.Persistence.Repositories
{
    public class Repository<T, TKey> : IRepository<T, TKey>
        where T : AggregateRoot<TKey>
    {
        private readonly AdsDbContext _dbContext;
        protected DbSet<T> DbSet => _dbContext.Set<T>();

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _dbContext;
            }
        }

        public Repository(AdsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void AddOrUpdate(T entity)
        {
            if (entity.Id.Equals(default(TKey)))
            {
                DbSet.Add(entity);
            }
        }

        public void Delete(T entity)
        {
            DbSet.Remove(entity);
        }

        public IQueryable<T> GetAll()
        {
            return _dbContext.Set<T>();
        }
    }
}
