﻿using ClassifiedAds.Application;
using ClassifiedAds.Modules.Product.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClassifiedAds.Modules.Product.Queries
{
    public class GetProductsQuery : IQuery<List<Entities.Product>>
    {
    }

    public class GetProductsQueryHandler : IQueryHandler<GetProductsQuery, List<Entities.Product>>
    {
        private readonly IProductRepository _productRepository;

        public GetProductsQueryHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public Task<List<Entities.Product>> HandleAsync(GetProductsQuery query)
        {
            return _productRepository.ToListAsync(_productRepository.GetAll());
        }
    }
}
