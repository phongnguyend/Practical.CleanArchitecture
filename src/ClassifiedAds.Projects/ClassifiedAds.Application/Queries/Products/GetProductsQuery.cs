using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassifiedAds.Application.Queries.Products
{
    public class GetProductsQuery : IQuery<List<Product>>
    {
    }

    internal class GetProductsQueryHandler : IQueryHandler<GetProductsQuery, List<Product>>
    {
        private readonly IRepository<Product, Guid> _productRepository;

        public GetProductsQueryHandler(IRepository<Product, Guid> productRepository)
        {
            _productRepository = productRepository;
        }

        public List<Product> Handle(GetProductsQuery query)
        {
            return _productRepository.GetAll().ToList();
        }
    }
}
