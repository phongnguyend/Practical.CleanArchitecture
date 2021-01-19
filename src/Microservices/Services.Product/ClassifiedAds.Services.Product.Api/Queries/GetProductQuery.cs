﻿using ClassifiedAds.Application;
using ClassifiedAds.CrossCuttingConcerns.Exceptions;
using ClassifiedAds.Services.Product.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Product.Queries
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

        public async Task<Entities.Product> HandleAsync(GetProductQuery query)
        {
            var product = await _productRepository.FirstOrDefaultAsync(_productRepository.GetAll().Where(x => x.Id == query.Id));

            if (query.ThrowNotFoundIfNull && product == null)
            {
                throw new NotFoundException($"Product {query.Id} not found.");
            }

            return product;
        }
    }
}
