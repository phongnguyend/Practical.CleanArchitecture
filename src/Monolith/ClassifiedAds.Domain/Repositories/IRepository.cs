using ClassifiedAds.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Domain.Repositories
{
    public interface IRepository<TEntity, TKey>
        where TEntity : AggregateRoot<TKey>
    {
        IUnitOfWork UnitOfWork { get; }

        IQueryable<TEntity> GetAll();

        Task AddOrUpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

        void Delete(TEntity entity);

        Task<T> FirstOrDefaultAsync<T>(IQueryable<T> query);

        Task<T> SingleOrDefaultAsync<T>(IQueryable<T> query);

        Task<List<T>> ToListAsync<T>(IQueryable<T> query);

        void BulkInsert(IEnumerable<TEntity> entities);

        void BulkInsert(IEnumerable<TEntity> entities, Expression<Func<TEntity, object>> columnNamesSelector);

        void BulkUpdate(IEnumerable<TEntity> entities, Expression<Func<TEntity, object>> columnNamesSelector);

        void BulkMerge(IEnumerable<TEntity> entities, Expression<Func<TEntity, object>> idSelector, Expression<Func<TEntity, object>> updateColumnNamesSelector, Expression<Func<TEntity, object>> insertColumnNamesSelector);

        void BulkDelete(IEnumerable<TEntity> entities);
    }
}
