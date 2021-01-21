using ClassifiedAds.Application.AuditLogEntries.DTOs;
using ClassifiedAds.Domain.Entities;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private async Task<List<Product>> GetProductsAsync()
        {
            var products = await GetAsync<List<Product>>("api/products");
            return products;
        }

        private async Task<Product> GetProductByIdAsync(Guid id)
        {
            var product = await GetAsync<Product>($"api/products/{id}");
            return product;
        }

        private async Task<Product> CreateProductAsync(Product product)
        {
            var policy = Policy.Handle<Exception>().RetryAsync(5);

            return await policy.ExecuteAsync(async () =>
             {
                 var createdProduct = await PostAsync<Product>("api/products", product);
                 return createdProduct;
             });

        }

        private async Task<Product> UpdateProductAsync(Guid id, Product product)
        {
            var updatedProduct = await PutAsync<Product>($"api/products/{id}", product);
            return updatedProduct;
        }

        private async Task DeleteProductAsync(Guid id)
        {
            await DeleteAsync($"api/products/{id}");
        }

        public async Task<List<AuditLogEntryDTO>> GetAuditLogsAsync(Guid id)
        {
            var auditLogs = await GetAsync<List<AuditLogEntryDTO>>($"api/products/{id}/auditlogs");
            return auditLogs;
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
            Product createdProduct = await CreateProductAsync(product);
            Assert.True(product.Id != createdProduct.Id);
            Assert.Equal(product.Name, createdProduct.Name);
            Assert.Equal(product.Code, createdProduct.Code);
            Assert.Equal(product.Description, createdProduct.Description);

            var auditLogs = await GetAuditLogsAsync(createdProduct.Id);
            Assert.Single(auditLogs);
            Assert.Contains(auditLogs, x => x.Action == "CREATED");

            // GET
            var products = await GetProductsAsync();
            Assert.True(products.Count > 0);

            // GET ONE
            var refreshedProduct = await GetProductByIdAsync(createdProduct.Id);
            Assert.Equal(refreshedProduct.Id, createdProduct.Id);
            Assert.Equal(refreshedProduct.Name, createdProduct.Name);
            Assert.Equal(refreshedProduct.Code, createdProduct.Code);
            Assert.Equal(refreshedProduct.Description, createdProduct.Description);

            // PUT
            refreshedProduct.Name = "Test 2";
            var updatedProduct = await UpdateProductAsync(refreshedProduct.Id, refreshedProduct);
            Assert.Equal(refreshedProduct.Id, updatedProduct.Id);
            Assert.Equal("Test 2", updatedProduct.Name);
            Assert.Equal(refreshedProduct.Code, updatedProduct.Code);
            Assert.Equal(refreshedProduct.Description, updatedProduct.Description);

            auditLogs = await GetAuditLogsAsync(createdProduct.Id);
            Assert.Equal(2, auditLogs.Count);
            Assert.Single(auditLogs, x => x.Action == "CREATED");
            Assert.Equal(1, auditLogs.Count(x => x.Action == "UPDATED"));

            // DELETE
            await DeleteProductAsync(createdProduct.Id);
            await Assert.ThrowsAsync<HttpRequestException>(async () => await GetProductByIdAsync(createdProduct.Id));
        }

    }
}
