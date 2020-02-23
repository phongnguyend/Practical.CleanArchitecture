using System;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Identity;
using ClassifiedAds.Domain.Repositories;

namespace ClassifiedAds.Domain.Services
{
    public class ProductService : CrudService<Product>, IProductService
    {
        public ProductService(IUnitOfWork unitOfWork, IRepository<Product, Guid> productRepository, ICurrentUser currentUser)
            : base(unitOfWork, productRepository)
        {
        }
    }
}
