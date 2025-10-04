using ClassifiedAds.Domain.Repositories;
using System;

namespace ClassifiedAds.Modules.Product.Persistence;

public interface IProductRepository : IRepository<Entities.Product, Guid>
{
}
