using ClassifiedAds.CrossCuttingConcerns.OS;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Modules.Identity.Repositories
{
    public class Repository<T, TKey> : IRepository<T, TKey>
        where T : AggregateRoot<TKey>
    {
        private readonly IdentityDbContext _dbContext;
        private readonly IDateTimeProvider _dateTimeProvider;

        protected DbSet<T> DbSet => _dbContext.Set<T>();

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _dbContext;
            }
        }

        public Repository(IdentityDbContext dbContext, IDateTimeProvider dateTimeProvider)
        {
            _dbContext = dbContext;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task AddOrUpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            if (entity.Id.Equals(default(TKey)))
            {
                entity.CreatedDateTime = _dateTimeProvider.OffsetNow;
                await DbSet.AddAsync(entity, cancellationToken);
            }
            else
            {
                entity.UpdatedDateTime = _dateTimeProvider.OffsetNow;
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

        public Task<T1> FirstOrDefaultAsync<T1>(IQueryable<T1> query)
        {
            return query.FirstOrDefaultAsync();
        }

        public Task<T1> SingleOrDefaultAsync<T1>(IQueryable<T1> query)
        {
            return query.SingleOrDefaultAsync();
        }

        public Task<List<T1>> ToListAsync<T1>(IQueryable<T1> query)
        {
            return query.ToListAsync();
        }
    }
}
