using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Identity;
using ClassifiedAds.Domain.Repositories;
using System;

namespace ClassifiedAds.Application.Products.Services;

public class ProductService : CrudService<Product>, IProductService
{
    public ProductService(IRepository<Product, Guid> productRepository, Dispatcher dispatcher, ICurrentUser currentUser)
        : base(productRepository, dispatcher)
    {
    }
}
