using ClassifiedAds.CrossCuttingConcerns.DateTimes;
using System;

namespace ClassifiedAds.Services.Product.Persistence;

public class ProductRepository : Repository<Entities.Product, Guid>, IProductRepository
{
    public ProductRepository(ProductDbContext dbContext, IDateTimeProvider dateTimeProvider)
        : base(dbContext, dateTimeProvider)
    {
    }
}
