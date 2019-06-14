
using System;
using System.Collections.Generic;

namespace ClassifiedAds.DomainServices.Services
{
    public interface IGenericService<T>
    {
        IList<T> Get();

        T GetById(Guid guid);

        void Add(T entity);

        void Update(T entity);
    }
}
