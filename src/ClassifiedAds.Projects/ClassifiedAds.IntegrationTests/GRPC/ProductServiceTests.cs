using ClassifiedAds.GRPC;
using Grpc.Net.Client;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace ClassifiedAds.IntegrationTests.GRPC
{
    public class ProductServiceTests
    {
        private readonly Product.ProductClient _client;

        public ProductServiceTests()
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001",
                new GrpcChannelOptions
                {
                    HttpClient = new HttpClient(new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                        {
                            // TODO: verify the Certificate
                            return true;
                        }
                    })
                });
            _client = new Product.ProductClient(channel);
        }

        [Fact]
        public async Task GetProducts()
        {
            var reply = await _client.GetProductsAsync(new GetProductsRequest());
        }
    }
}
