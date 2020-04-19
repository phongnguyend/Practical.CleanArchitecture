using System;

namespace ClassifiedAds.Modules.Product.Repositories
{
    public class ProductRepository : Repository<Entities.Product, Guid>, IProductRepository
    {
        public ProductRepository(ProductDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
