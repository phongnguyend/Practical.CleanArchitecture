using ClassifiedAds.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Domain.Repositories;

public interface IRepository<TEntity, TKey> : IConcurrencyHandler<TEntity>
    where TEntity : Entity<TKey>, IAggregateRoot
{
    IUnitOfWork UnitOfWork { get; }

    IQueryable<TEntity> GetQueryableSet();

    Task AddOrUpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    void Delete(TEntity entity);

    Task<T> FirstOrDefaultAsync<T>(IQueryable<T> query);

    Task<T> SingleOrDefaultAsync<T>(IQueryable<T> query);

    Task<List<T>> ToListAsync<T>(IQueryable<T> query);

    Task BulkInsertAsync(IReadOnlyCollection<TEntity> entities, CancellationToken cancellationToken = default);

    Task BulkInsertAsync(IReadOnlyCollection<TEntity> entities, Expression<Func<TEntity, object>> columnNamesSelector, CancellationToken cancellationToken = default);

    Task BulkUpdateAsync(IReadOnlyCollection<TEntity> entities, Expression<Func<TEntity, object>> columnNamesSelector, CancellationToken cancellationToken = default);

    Task BulkMergeAsync(IReadOnlyCollection<TEntity> entities, Expression<Func<TEntity, object>> idSelector, Expression<Func<TEntity, object>> updateColumnNamesSelector, Expression<Func<TEntity, object>> insertColumnNamesSelector, CancellationToken cancellationToken = default);

    Task BulkDeleteAsync(IReadOnlyCollection<TEntity> entities, CancellationToken cancellationToken = default);
}
