using ClassifiedAds.CrossCuttingConcerns.DateTimes;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Repositories;
using EntityFrameworkCore.SqlServer.SimpleBulks.BulkDelete;
using EntityFrameworkCore.SqlServer.SimpleBulks.BulkInsert;
using EntityFrameworkCore.SqlServer.SimpleBulks.BulkMerge;
using EntityFrameworkCore.SqlServer.SimpleBulks.BulkUpdate;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Persistence.Repositories
{
    public class Repository<T, TKey> : IRepository<T, TKey>
        where T : Entity<TKey>, IAggregateRoot
    {
        private readonly AdsDbContext _dbContext;
        private readonly IDateTimeProvider _dateTimeProvider;

        protected DbSet<T> DbSet => _dbContext.Set<T>();

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _dbContext;
            }
        }

        public Repository(AdsDbContext dbContext, IDateTimeProvider dateTimeProvider)
        {
            _dbContext = dbContext;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task AddOrUpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            if (entity.Id.Equals(default(TKey)))
            {
                await AddAsync(entity, cancellationToken);
            }
            else
            {
                await UpdateAsync(entity, cancellationToken);
            }
        }

        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            entity.CreatedDateTime = _dateTimeProvider.OffsetNow;
            await DbSet.AddAsync(entity, cancellationToken);
        }

        public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            entity.UpdatedDateTime = _dateTimeProvider.OffsetNow;
            return Task.CompletedTask;
        }

        public void Delete(T entity)
        {
            DbSet.Remove(entity);
        }

        public IQueryable<T> GetQueryableSet()
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

        public void BulkInsert(IEnumerable<T> entities)
        {
            _dbContext.BulkInsert(entities);
        }

        public void BulkInsert(IEnumerable<T> entities, Expression<Func<T, object>> columnNamesSelector)
        {
            _dbContext.BulkInsert(entities, columnNamesSelector);
        }

        public void BulkUpdate(IEnumerable<T> entities, Expression<Func<T, object>> columnNamesSelector)
        {
            _dbContext.BulkUpdate(entities, columnNamesSelector);
        }

        public void BulkDelete(IEnumerable<T> entities)
        {
            _dbContext.BulkDelete(entities);
        }

        public void BulkMerge(IEnumerable<T> entities, Expression<Func<T, object>> idSelector, Expression<Func<T, object>> updateColumnNamesSelector, Expression<Func<T, object>> insertColumnNamesSelector)
        {
            _dbContext.BulkMerge(entities, idSelector, updateColumnNamesSelector, insertColumnNamesSelector);
        }

        public void SetRowVersion(T entity, byte[] version)
        {
            _dbContext.Entry(entity).OriginalValues[nameof(entity.RowVersion)] = version;
        }

        public bool IsDbUpdateConcurrencyException(Exception ex)
        {
            return ex is DbUpdateConcurrencyException;
        }
    }
}
