using ClassifiedAds.Domain.Services;
using Grpc.Core;
using Microsoft.Extensions.Logging;
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
    }
}
