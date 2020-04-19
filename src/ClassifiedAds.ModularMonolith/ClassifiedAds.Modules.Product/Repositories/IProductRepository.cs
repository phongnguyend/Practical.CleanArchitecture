using System;

namespace ClassifiedAds.Modules.Product.Repositories
{
    public interface IProductRepository : IRepository<Entities.Product, Guid>
    {
    }
}
