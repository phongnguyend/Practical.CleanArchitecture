using ClassifiedAds.Domain.Repositories;
using System;

namespace ClassifiedAds.Services.Product.Repositories;

public interface IProductRepository : IRepository<Entities.Product, Guid>
{
}
