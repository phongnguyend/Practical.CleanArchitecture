using ClassifiedAds.Application;
using ClassifiedAds.CrossCuttingConcerns.Exceptions;
using ClassifiedAds.Modules.Product.Repositories;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Modules.Product.Queries;

public class GetProductQuery : IQuery<Entities.Product>
{
    public Guid Id { get; set; }
    public bool ThrowNotFoundIfNull { get; set; }
}

public class GetProductQueryHandler : IQueryHandler<GetProductQuery, Entities.Product>
{
    private readonly IProductRepository _productRepository;

    public GetProductQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Entities.Product> HandleAsync(GetProductQuery query, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.FirstOrDefaultAsync(_productRepository.GetQueryableSet().Where(x => x.Id == query.Id));

        if (query.ThrowNotFoundIfNull && product == null)
        {
            throw new NotFoundException($"Product {query.Id} not found.");
        }

        return product;
    }
}
