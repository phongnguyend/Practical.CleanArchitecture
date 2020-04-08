using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Repositories;
using System;

namespace ClassifiedAds.Domain.Services
{
    public class StoreService : CrudService<Store>, IStoreService
    {
        private readonly IStoreRepository _storeRepository;

        public StoreService(IStoreRepository storeRepository)
            : base(storeRepository)
        {
            _storeRepository = storeRepository;
        }

        public Store GetStoreIncludeProducts(Guid Id)
        {
            return _storeRepository.GetStoreIncludeProducts(Id);
        }
    }
}
