using ClassifiedAds.DomainServices.Entities;
using System;

namespace ClassifiedAds.DomainServices.Services
{
    public interface IStoreService : ICrudService<Store>
    {
        Store GetStoreIncludeProducts(Guid Id);
    }
}
