using ClassifiedAds.DomainServices.Entities;
using ClassifiedAds.DomainServices.Repositories;
using System;

namespace ClassifiedAds.DomainServices.Services
{
    public class StoreService : GenericService<Store>, IStoreService
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
