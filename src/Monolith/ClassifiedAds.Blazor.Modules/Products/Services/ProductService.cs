using ClassifiedAds.Blazor.Modules.Core.Services;
using ClassifiedAds.Blazor.Modules.Products.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClassifiedAds.Blazor.Modules.Products.Services;

public class ProductService : HttpService
{
    public ProductService(HttpClient httpClient, ITokenManager tokenManager)
        : base(httpClient, tokenManager)
    {
    }

    public async Task<List<ProductModel>> GetProductsAsync()
    {
        var products = await GetAsync<List<ProductModel>>("api/products");
        return products;
    }

    public async Task<ProductModel> GetProductByIdAsync(Guid id)
    {
        var product = await GetAsync<ProductModel>($"api/products/{id}");
        return product;
    }

    public async Task<ProductModel> CreateProductAsync(ProductModel product)
    {
        var createdProduct = await PostAsync<ProductModel>("api/products", product);
        return createdProduct;
    }

    public async Task<ProductModel> UpdateProductAsync(Guid id, ProductModel product)
    {
        var updatedProduct = await PutAsync<ProductModel>($"api/products/{id}", product);
        return updatedProduct;
    }

    public async Task DeleteProductAsync(Guid id)
    {
        await DeleteAsync($"api/products/{id}");
    }

    public async Task<List<ProductAuditLogModel>> GetAuditLogsAsync(Guid id)
    {
        var auditLogs = await GetAsync<List<ProductAuditLogModel>>($"api/products/{id}/auditlogs");
        return auditLogs;
    }

    public string GetExportPdfUrl()
    {
        return $"{_httpClient.BaseAddress.AbsoluteUri.Trim('/')}/api/products/ExportAsPdf";
    }

    public string GetExportCsvUrl()
    {
        return $"{_httpClient.BaseAddress.AbsoluteUri.Trim('/')}/api/products/ExportAsCsv";
    }

    public async Task<List<ProductModel>> VectorSearchProductsAsync(string searchText)
    {
        var products = await GetAsync<List<ProductModel>>($"api/products/vectorsearch?searchText={searchText}");
        return products;
    }
}
