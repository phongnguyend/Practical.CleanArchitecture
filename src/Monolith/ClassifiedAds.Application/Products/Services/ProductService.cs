using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Events;
using ClassifiedAds.Domain.Identity;
using ClassifiedAds.Domain.Repositories;
using System;

namespace ClassifiedAds.Application.Products.Services
{
    public class ProductService : CrudService<Product>, IProductService
    {
        public ProductService(IRepository<Product, Guid> productRepository, IDomainEvents domainEvents, ICurrentUser currentUser)
            : base(productRepository, domainEvents)
        {
        }
    }
}
