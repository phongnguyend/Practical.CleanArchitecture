using System.Linq;
using ClassifiedAds.DomainServices;
using Microsoft.EntityFrameworkCore;

namespace ClassifiedAds.Persistence.Repositories
{
    public class Repository<T> : IRepository<T>
        where T : class
    {
        private readonly AdsDbContext _dbContext;
        protected DbSet<T> DbSet => _dbContext.Set<T>();

        public Repository(AdsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(T entity)
        {
            DbSet.Add(entity);
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
