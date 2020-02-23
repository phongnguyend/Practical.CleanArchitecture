using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Repositories;
using System;

namespace ClassifiedAds.Domain.Services
{
    public class StoreService : CrudService<Store>, IStoreService
    {
        private readonly IStoreRepository _storeRepository;

        public StoreService(IUnitOfWork unitOfWork, IStoreRepository storeRepository)
            : base(unitOfWork, storeRepository)
        {
            _storeRepository = storeRepository;
        }

        public Store GetStoreIncludeProducts(Guid Id)
        {
            return _storeRepository.GetStoreIncludeProducts(Id);
        }
    }
}
