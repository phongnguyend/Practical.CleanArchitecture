using ClassifiedAds.CrossCuttingConcerns.DateTimes;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Repositories;
using EntityFrameworkCore.SqlServer.SimpleBulks.BulkDelete;
using EntityFrameworkCore.SqlServer.SimpleBulks.BulkInsert;
using EntityFrameworkCore.SqlServer.SimpleBulks.BulkMerge;
using EntityFrameworkCore.SqlServer.SimpleBulks.BulkUpdate;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ClassifiedAds.Persistence.SqlServer;

public class DbContextRepository<TDbContext, TEntity, TKey> : IRepository<TEntity, TKey>
    where TEntity : Entity<TKey>, IAggregateRoot
    where TDbContext : DbContext, IUnitOfWork
{
    private readonly TDbContext _dbContext;
    private readonly IDateTimeProvider _dateTimeProvider;

    protected DbSet<TEntity> DbSet => _dbContext.Set<TEntity>();

    public IUnitOfWork UnitOfWork
    {
        get
        {
            return _dbContext;
        }
    }

    public DbContextRepository(TDbContext dbContext, IDateTimeProvider dateTimeProvider)
    {
        _dbContext = dbContext;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task AddOrUpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
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

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        entity.CreatedDateTime = _dateTimeProvider.OffsetNow;
        await DbSet.AddAsync(entity, cancellationToken);
    }

    public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        entity.UpdatedDateTime = _dateTimeProvider.OffsetNow;
        return Task.CompletedTask;
    }

    public void Delete(TEntity entity)
    {
        DbSet.Remove(entity);
    }

    public IQueryable<TEntity> GetQueryableSet()
    {
        return _dbContext.Set<TEntity>();
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

    public async Task BulkInsertAsync(IReadOnlyCollection<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await _dbContext.BulkInsertAsync(entities, cancellationToken: cancellationToken);
    }

    public async Task BulkInsertAsync(IReadOnlyCollection<TEntity> entities, Expression<Func<TEntity, object>> columnNamesSelector, CancellationToken cancellationToken = default)
    {
        await _dbContext.BulkInsertAsync(entities, columnNamesSelector, cancellationToken: cancellationToken);
    }

    public async Task BulkUpdateAsync(IReadOnlyCollection<TEntity> entities, Expression<Func<TEntity, object>> columnNamesSelector, CancellationToken cancellationToken = default)
    {
        await _dbContext.BulkUpdateAsync(entities, columnNamesSelector, cancellationToken: cancellationToken);
    }

    public async Task BulkDeleteAsync(IReadOnlyCollection<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await _dbContext.BulkDeleteAsync(entities, cancellationToken: cancellationToken);
    }

    public async Task BulkMergeAsync(IReadOnlyCollection<TEntity> entities, Expression<Func<TEntity, object>> idSelector, Expression<Func<TEntity, object>> updateColumnNamesSelector, Expression<Func<TEntity, object>> insertColumnNamesSelector, CancellationToken cancellationToken = default)
    {
        await _dbContext.BulkMergeAsync(entities, idSelector, updateColumnNamesSelector, insertColumnNamesSelector, cancellationToken: cancellationToken);
    }

    public void SetRowVersion(TEntity entity, byte[] version)
    {
        _dbContext.Entry(entity).OriginalValues[nameof(entity.RowVersion)] = version;
    }

    public bool IsDbUpdateConcurrencyException(Exception ex)
    {
        return ex is DbUpdateConcurrencyException;
    }
}
