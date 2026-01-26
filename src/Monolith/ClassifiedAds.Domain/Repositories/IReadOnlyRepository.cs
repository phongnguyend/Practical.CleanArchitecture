using ClassifiedAds.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassifiedAds.Domain.Repositories;

public interface IReadOnlyRepository<TEntity, TKey>
    where TEntity : Entity<TKey>, IAggregateRoot
{
    IQueryable<TEntity> GetQueryableSet();

    Task<T> FirstOrDefaultAsync<T>(IQueryable<T> query);

    Task<T> SingleOrDefaultAsync<T>(IQueryable<T> query);

    Task<List<T>> ToListAsync<T>(IQueryable<T> query);
}
