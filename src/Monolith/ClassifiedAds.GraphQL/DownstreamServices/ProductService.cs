using ClassifiedAds.CrossCuttingConcerns.ExtensionMethods;
using ClassifiedAds.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ClassifiedAds.GraphQL.DownstreamServices
{
    public class ProductService
    {
        private readonly HttpClient _httpClient;

        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Product>> GetProducts()
        {
            SetBearerToken();

            var response = await _httpClient.GetAsync("api/products", HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var products = await response.Content.ReadAs<List<Product>>();
            return products;
        }

        private void SetBearerToken()
        {

        }

        public async Task<Product> GetProductById(Guid id)
        {
            SetBearerToken();

            var response = await _httpClient.GetAsync($"api/products/{id}", HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var product = await response.Content.ReadAs<Product>();
            return product;
        }

        public async Task<Product> CreateProduct(Product product)
        {
            SetBearerToken();

            var request = new HttpRequestMessage(HttpMethod.Post, "api/products");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            request.Content = product.AsJsonContent();

            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var createdProduct = await response.Content.ReadAs<Product>();
            return createdProduct;
        }

        public async Task<Product> UpdateProduct(Guid id, Product product)
        {
            SetBearerToken();

            var request = new HttpRequestMessage(HttpMethod.Put, $"api/products/{id}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            request.Content = product.AsJsonContent();

            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var updatedProduct = await response.Content.ReadAs<Product>();
            return updatedProduct;
        }

        public async Task DeleteProduct(Guid id)
        {
            SetBearerToken();

            var request = new HttpRequestMessage(HttpMethod.Delete, $"api/products/{id}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
        }
    }
}
