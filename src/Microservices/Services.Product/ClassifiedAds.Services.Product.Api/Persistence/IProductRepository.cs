using ClassifiedAds.Domain.Repositories;
using System;

namespace ClassifiedAds.Services.Product.Persistence;

public interface IProductRepository : IRepository<Entities.Product, Guid>
{
}
