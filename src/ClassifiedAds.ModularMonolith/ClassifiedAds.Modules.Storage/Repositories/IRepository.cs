using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Repositories;
using System.Linq;

namespace ClassifiedAds.Modules.Storage.Repositories
{
    public interface IRepository<TEntity, TKey>
        where TEntity : AggregateRoot<TKey>
    {
        IUnitOfWork UnitOfWork { get; }

        IQueryable<TEntity> GetAll();

        void AddOrUpdate(TEntity entity);

        void Delete(TEntity entity);
    }
}
