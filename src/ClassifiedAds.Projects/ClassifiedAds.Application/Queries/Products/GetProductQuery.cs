using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Services;
using System;

namespace ClassifiedAds.Application.Queries.Products
{
    public class GetProductQuery : IQuery<Product>
    {
        public Guid Id { get; set; }
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
            return _productService.GetById(query.Id);
        }
    }
}
