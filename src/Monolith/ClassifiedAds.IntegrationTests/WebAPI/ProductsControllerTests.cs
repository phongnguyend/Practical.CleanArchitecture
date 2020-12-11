using ClassifiedAds.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace ClassifiedAds.IntegrationTests.WebAPI
{
    public class ProductsControllerTests : TestBase
    {
        public ProductsControllerTests()
            : base()
        {
            _httpClient.Timeout = new TimeSpan(0, 0, 30);
            _httpClient.DefaultRequestHeaders.Clear();
        }

        private async Task<List<Product>> GetProducts()
        {
            var products = await GetAsync<List<Product>>("api/products");
            return products;
        }

        private async Task<Product> GetProductById(Guid id)
        {
            var product = await GetAsync<Product>($"api/products/{id}");
            return product;
        }

        private async Task<Product> CreateProduct(Product product)
        {
            var createdProduct = await PostAsync<Product>("api/products", product);
            return createdProduct;
        }

        private async Task<Product> UpdateProduct(Guid id, Product product)
        {
            var updatedProduct = await PutAsync<Product>($"api/products/{id}", product);
            return updatedProduct;
        }

        private async Task DeleteProduct(Guid id)
        {
            await DeleteAsync($"api/products/{id}");
        }

        [Fact]
        public async Task AllInOne()
        {
            await GetTokenAsync();

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
