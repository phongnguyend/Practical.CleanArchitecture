using ClassifiedAds.DomainServices.Entities;
using System;

namespace ClassifiedAds.DomainServices.Services
{
    public interface IStoreService : IGenericService<Store>
    {
        Store GetStoreIncludeProducts(Guid Id);
    }
}
