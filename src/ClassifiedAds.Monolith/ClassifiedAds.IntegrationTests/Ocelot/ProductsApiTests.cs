using ClassifiedAds.CrossCuttingConcerns.ExtensionMethods;
using ClassifiedAds.Domain.Entities;
using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace ClassifiedAds.IntegrationTests.Ocelot
{
    public class ProductsApiTests : TestBase
    {
        private static HttpClient _httpClient = new HttpClient();

        public ProductsApiTests()
            : base()
        {
            _httpClient.BaseAddress = new Uri(AppSettings.Ocelot.Endpoint);
            _httpClient.Timeout = new TimeSpan(0, 0, 30);
            _httpClient.DefaultRequestHeaders.Clear();
        }

        private async Task<List<Product>> GetProducts()
        {
            var response = await _httpClient.GetAsync("ocelot/products", HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var products = await response.Content.ReadAs<List<Product>>();
            return products;
        }

        private static async Task<Product> GetProductById(Guid id)
        {
            var response = await _httpClient.GetAsync($"ocelot/products/{id}", HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var product = await response.Content.ReadAs<Product>();
            return product;
        }

        private static async Task<Product> CreateProduct(Product product)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "ocelot/products");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            request.Content = product.AsJsonContent();

            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var createdProduct = await response.Content.ReadAs<Product>();
            return createdProduct;
        }

        private static async Task<Product> UpdateProduct(Guid id, Product product)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, $"ocelot/products/{id}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            request.Content = product.AsJsonContent();

            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var updatedProduct = await response.Content.ReadAs<Product>();
            return updatedProduct;
        }

        private static async Task DeleteProduct(Guid id)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"ocelot/products/{id}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task AllInOne()
        {
            var metaDataResponse = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = AppSettings.OpenIdConnect.Authority,
                Policy = { RequireHttps = AppSettings.OpenIdConnect.RequireHttpsMetadata },
            });

            var tokenResponse = await _httpClient.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = metaDataResponse.TokenEndpoint,
                ClientId = AppSettings.OpenIdConnect.ClientId,
                ClientSecret = AppSettings.OpenIdConnect.ClientSecret,
                UserName = AppSettings.Login.UserName,
                Password = AppSettings.Login.Password,
                Scope = AppSettings.Login.Scope,
            });

            var token = tokenResponse.AccessToken;
            _httpClient.UseBearerToken(token);

            // POST
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

            // GET
            var products = await GetProducts();
            Assert.True(products.Count > 0);

            // GET ONE
            var refreshedProduct = await GetProductById(createdProduct.Id);
            Assert.Equal(refreshedProduct.Id, createdProduct.Id);
            Assert.Equal(refreshedProduct.Name, createdProduct.Name);
            Assert.Equal(refreshedProduct.Code, createdProduct.Code);
            Assert.Equal(refreshedProduct.Description, createdProduct.Description);

            // PUT
            refreshedProduct.Name = "Test 2";
            var updatedProduct = await UpdateProduct(refreshedProduct.Id, refreshedProduct);
            Assert.Equal(refreshedProduct.Id, updatedProduct.Id);
            Assert.Equal("Test 2", updatedProduct.Name);
            Assert.Equal(refreshedProduct.Code, updatedProduct.Code);
            Assert.Equal(refreshedProduct.Description, updatedProduct.Description);

            // DELETE
            await DeleteProduct(createdProduct.Id);
            await Assert.ThrowsAsync<HttpRequestException>(async () => await GetProductById(createdProduct.Id));
        }

    }
}
