using ClassifiedAds.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClassifiedAds.Application
{
    public interface ICrudService<T>
        where T : AggregateRoot<Guid>
    {
        Task<List<T>> GetAsync();

        Task<T> GetByIdAsync(Guid guid);

        Task AddOrUpdateAsync(T entity);

        Task DeleteAsync(T entity);
    }
}
