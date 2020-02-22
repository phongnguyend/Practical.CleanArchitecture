using System;
using ClassifiedAds.DomainServices.Entities;
using ClassifiedAds.DomainServices.Identity;
using ClassifiedAds.DomainServices.Repositories;

namespace ClassifiedAds.DomainServices.Services
{
    public class ProductService : CrudService<Product>, IProductService
    {
        public ProductService(IUnitOfWork unitOfWork, IRepository<Product, Guid> productRepository, ICurrentUser currentUser)
            : base(unitOfWork, productRepository)
        {
        }
    }
}
