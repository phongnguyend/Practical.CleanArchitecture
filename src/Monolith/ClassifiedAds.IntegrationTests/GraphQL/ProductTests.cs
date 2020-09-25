using ClassifiedAds.Domain.Entities;
using GraphQL.Client;
using GraphQL.Common.Request;
using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace ClassifiedAds.IntegrationTests.GraphQL
{
    public class ProductTests : TestBase
    {
        private readonly GraphQLClient _client;

        public ProductTests()
            : base()
        {
            _client = new GraphQLClient(AppSettings.GraphQL.Endpoint);
        }

        private async Task<List<Product>> GetProducts()
        {
            var query = new GraphQLRequest
            {
                Query = @"
                {
                    products
                    {
                        id
                        code
                        name
                        description
                    }
                }",
            };

            var response = await _client.PostAsync(query);
            return response.GetDataFieldAs<List<Product>>("products");
        }

        private async Task<Product> GetProductById(Guid id)
        {
            var query = new GraphQLRequest
            {
                Query = @"
                query productQuery($productId: ID!)
                {   
                    product(id: $productId) 
                    {
                        id
                        code
                        name
                        description
                    }
                }",
                Variables = new { productId = id },
            };
            var response = await _client.PostAsync(query);
            return response.GetDataFieldAs<Product>("product");
        }

        private async Task<Product> CreateProduct(Product product)
        {
            var query = new GraphQLRequest
            {
                Query = @" 
                mutation($product: ProductInput!)
                {
                    createProduct(product: $product)
                    {
                        id 
                        code 
                        name 
                        description
                    }
                }",
                Variables = new { product = new { product.Code, product.Name, product.Description } },
            };
            var response = await _client.PostAsync(query);
            return response.GetDataFieldAs<Product>("createProduct");
        }

        private async Task DeleteProduct(Guid id)
        {
            var query = new GraphQLRequest
            {
                Query = @" 
                mutation($productId: ID!)
                {
                    deleteProduct(id: $productId)
                }",
                Variables = new { productId = id },
            };
            var response = await _client.PostAsync(query);
            var rs = response.GetDataFieldAs<bool>("deleteProduct");
        }

        [Fact]
        public async Task AllInOne()
        {
            var httpClient = new HttpClient();
            var metaDataResponse = await httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = AppSettings.OpenIdConnect.Authority,
                Policy = { RequireHttps = AppSettings.OpenIdConnect.RequireHttpsMetadata },
            });

            var tokenResponse = await httpClient.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = metaDataResponse.TokenEndpoint,
                ClientId = AppSettings.OpenIdConnect.ClientId,
                ClientSecret = AppSettings.OpenIdConnect.ClientSecret,
                UserName = AppSettings.Login.UserName,
                Password = AppSettings.Login.Password,
                Scope = AppSettings.Login.Scope,
            });

            var token = tokenResponse.AccessToken;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var product = new Product
            {
                Name = "Test",
                Code = "TEST",
                Description = "Description",
            };

            Product createdProduct = await CreateProduct(product);
            Assert.True(product.Id != createdProduct.Id);
            Assert.Equal(product.Name, createdProduct.Name);
            Assert.Equal(product.Code, createdProduct.Code);
            Assert.Equal(product.Description, createdProduct.Description);

            var products = await GetProducts();
            Assert.True(products.Count > 0);

            var refreshedProduct = await GetProductById(createdProduct.Id);
            Assert.Equal(refreshedProduct.Id, createdProduct.Id);
            Assert.Equal(refreshedProduct.Name, createdProduct.Name);
            Assert.Equal(refreshedProduct.Code, createdProduct.Code);
            Assert.Equal(refreshedProduct.Description, createdProduct.Description);

            await DeleteProduct(createdProduct.Id);
            Assert.Null(await GetProductById(createdProduct.Id));
        }
    }
}
