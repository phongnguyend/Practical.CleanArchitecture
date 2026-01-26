using ClassifiedAds.CrossCuttingConcerns.DateTimes;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassifiedAds.Persistence.Repositories;

public class ReadOnlyRepository<TEntity, TKey> : IReadOnlyRepository<TEntity, TKey>
    where TEntity : Entity<TKey>, IAggregateRoot
{
    private AdsReadOnlyDbContext _dbContext;
    private IDateTimeProvider _dateTimeProvider;

    public ReadOnlyRepository(AdsReadOnlyDbContext dbContext, IDateTimeProvider dateTimeProvider)
    {
        _dbContext = dbContext;
        _dateTimeProvider = dateTimeProvider;
    }

    public IQueryable<TEntity> GetQueryableSet()
    {
        return _dbContext.Set<TEntity>();
    }

    public Task<T> FirstOrDefaultAsync<T>(IQueryable<T> query)
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
