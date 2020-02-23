using ClassifiedAds.Domain.Entities;
using System;

namespace ClassifiedAds.Domain.Repositories
{
    public interface IStoreRepository : IRepository<Store, Guid>
    {
        Store GetStoreIncludeProducts(Guid Id);
    }
}
