using ClassifiedAds.CrossCuttingConcerns.Exceptions;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Services;
using System;

namespace ClassifiedAds.Application.Queries.Products
{
    public class GetProductQuery : IQuery<Product>
    {
        public Guid Id { get; set; }
        public bool ThrowNotFoundIfNull { get; set; }
    }

    internal class GetProductQueryHandler : IQueryHandler<GetProductQuery, Product>
    {
        private readonly IProductService _productService;

        public GetProductQueryHandler(IProductService productService)
        {
            _productService = productService;
        }

        public Product Handle(GetProductQuery query)
        {
            var product = _productService.GetById(query.Id);

            if (query.ThrowNotFoundIfNull && product == null)
            {
                throw new NotFoundException($"Product {query.Id} not found.");
            }

            return product;
        }
    }
}
