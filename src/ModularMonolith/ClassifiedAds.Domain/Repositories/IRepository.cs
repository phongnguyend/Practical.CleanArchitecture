using ClassifiedAds.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
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
    }
}
