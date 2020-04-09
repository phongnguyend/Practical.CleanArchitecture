using ClassifiedAds.Domain.Services;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ClassifiedAds.GRPC
{
    public class ProductService : Product.ProductBase
    {
        private readonly ILogger<ProductService> _logger;
        private readonly IProductService _productService;

        public ProductService(ILogger<ProductService> logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        public override Task<GetProductsResponse> GetProducts(GetProductsRequest request, ServerCallContext context)
        {
            var products = _productService.Get().Select(x => new ProductMessage
            {
                Id = x.Id.ToString(),
                Code = x.Code,
                Name = x.Name,
                Description = x.Description
            });

            var rp = new GetProductsResponse();
            rp.Products.AddRange(products);
            return Task.FromResult(rp);
        }

        public override Task<GetProductResponse> GetProduct(GetProductRequest request, ServerCallContext context)
        {
            var product = _productService.GetById(Guid.Parse(request.Id));
            var response = new GetProductResponse
            {
                Product = product != null ? new ProductMessage
                {
                    Id = product.Id.ToString(),
                    Code = product.Code,
                    Name = product.Name,
                    Description = product.Description
                } : null
            };

            return Task.FromResult(response);
        }
    }
}
