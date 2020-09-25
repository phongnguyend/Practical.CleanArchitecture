using ClassifiedAds.Application;
using ClassifiedAds.CrossCuttingConcerns.Exceptions;
using ClassifiedAds.Modules.Product.Repositories;
using System;
using System.Linq;

namespace ClassifiedAds.Modules.Product.Queries
{
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

        public Entities.Product Handle(GetProductQuery query)
        {
            var product = _productRepository.GetAll().FirstOrDefault(x => x.Id == query.Id);

            if (query.ThrowNotFoundIfNull && product == null)
            {
                throw new NotFoundException($"Product {query.Id} not found.");
            }

            return product;
        }
    }
}
