using ClassifiedAds.Domain.Entities;
using System;

namespace ClassifiedAds.Domain.Services
{
    public interface IStoreService : ICrudService<Store>
    {
        Store GetStoreIncludeProducts(Guid Id);
    }
}
