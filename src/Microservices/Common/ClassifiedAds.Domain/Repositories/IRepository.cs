using ClassifiedAds.Domain.Entities;
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

        void AddOrUpdate(TEntity entity);

        Task AddOrUpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

        void Delete(TEntity entity);
    }
}
