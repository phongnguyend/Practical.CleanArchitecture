using Grpc.Net.Client;
using System.Threading.Tasks;
using Xunit;

namespace ClassifiedAds.GRPC.IntegrationTests
{
    public class ProductServiceTests
    {
        private readonly Product.ProductClient _client;

        public ProductServiceTests()
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            _client = new Product.ProductClient(channel);
        }

        [Fact]
        public async Task GetProducts()
        {
            var reply = await _client.GetProductsAsync(new GetProductsRequest());
        }
    }
}
