using ClassifiedAds.ApiIntegrationTests.ExtensionMethods;
using ClassifiedAds.ApiIntegrationTests.Models.AuditLogEntries;
using ClassifiedAds.ApiIntegrationTests.Models.Products;
using Polly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace ClassifiedAds.ApiIntegrationTests.WebAPI;

public class ProductsControllerTests : TestBase
{
    public ProductsControllerTests()
        : base()
    {
        _httpClient.Timeout = new TimeSpan(0, 0, 30);
        _httpClient.DefaultRequestHeaders.Clear();
    }

    private async Task<List<ProductModel>> GetProductsAsync()
    {
        var products = await GetAsync<List<ProductModel>>("api/products");
        return products;
    }

    private async Task<ProductModel> GetProductByIdAsync(Guid id)
    {
        var product = await GetAsync<ProductModel>($"api/products/{id}");
        return product;
    }

    private async Task<ProductModel> CreateProductAsync(ProductModel product)
    {
        var policy = Policy.Handle<Exception>().RetryAsync(5);

        return await policy.ExecuteAsync(async () =>
         {
             var createdProduct = await PostAsync<ProductModel>("api/products", product);
             return createdProduct;
         });

    }

    private async Task<ProductModel> UpdateProductAsync(Guid id, ProductModel product)
    {
        var updatedProduct = await PutAsync<ProductModel>($"api/products/{id}", product);
        return updatedProduct;
    }

    private async Task DeleteProductAsync(Guid id)
    {
        await DeleteAsync($"api/products/{id}");
    }

    public async Task<List<AuditLogEntryModel>> GetAuditLogsAsync(Guid id)
    {
        var auditLogs = await GetAsync<List<AuditLogEntryModel>>($"api/products/{id}/auditlogs");
        return auditLogs;
    }

    private async Task ExportAsPdfAsync(string path, string fileName)
    {
        using var response = await _httpClient.GetAsync($"api/products/ExportAsPdf");
        using var fileStream = new FileStream(Path.Combine(path, fileName), FileMode.CreateNew);
        await response.Content.CopyToAsync(fileStream);
    }

    private async Task ExportAsCsvAsync(string path, string fileName)
    {
        using var response = await _httpClient.GetAsync($"api/products/ExportAsCsv");
        using var fileStream = new FileStream(Path.Combine(path, fileName), FileMode.CreateNew);
        await response.Content.CopyToAsync(fileStream);
    }

    private async Task<List<ProductModel>> ImportCsvAsync(string filePath)
    {
        using var form = new MultipartFormDataContent();
        using var fileContent = new ByteArrayContent(File.ReadAllBytes(filePath));
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
        form.Add(fileContent, "formFile", "Products.csv");
        form.Add(new StringContent("Products.csv"), "name");

        var response = await _httpClient.PostAsync($"api/products/ImportCsv", form);
        response.EnsureSuccessStatusCode();

        var products = await response.Content.ReadAs<List<ProductModel>>();
        return products;
    }

    [Fact]
    public async Task AllInOne()
    {
        await GetTokenAsync();

        // POST
        var product = new ProductModel
        {
            Name = "Test",
            Code = "TEST",
            Description = "Description",
        };
        ProductModel createdProduct = await CreateProductAsync(product);
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
        refreshedProduct.Name = "Test' 2,2 \"a\"";
        var updatedProduct = await UpdateProductAsync(refreshedProduct.Id, refreshedProduct);
        Assert.Equal(refreshedProduct.Id, updatedProduct.Id);
        Assert.Equal("Test' 2,2 \"a\"", updatedProduct.Name);
        Assert.Equal(refreshedProduct.Code, updatedProduct.Code);
        Assert.Equal(refreshedProduct.Description, updatedProduct.Description);

        auditLogs = await GetAuditLogsAsync(createdProduct.Id);
        Assert.Equal(2, auditLogs.Count);
        Assert.Single(auditLogs, x => x.Action == "CREATED");
        Assert.Equal(1, auditLogs.Count(x => x.Action == "UPDATED"));

        // EXPORT PDF
        var path = Path.Combine(AppSettings.DownloadsFolder, "Practical.CleanArchitecture", Guid.NewGuid().ToString());
        Directory.CreateDirectory(path);

        await ExportAsPdfAsync(path, "Products.pdf");
        Assert.True(File.Exists(Path.Combine(path, "Products.pdf")));

        // EXPORT CSV
        await ExportAsCsvAsync(path, "Products.csv");
        Assert.True(File.Exists(Path.Combine(path, "Products.csv")));

        // IMPORT CSV
        var importingProducts = await ImportCsvAsync(Path.Combine(path, "Products.csv"));
        Assert.True(importingProducts.Count > 0);

        // DELETE
        await DeleteProductAsync(createdProduct.Id);
        await Assert.ThrowsAsync<HttpRequestException>(async () => await GetProductByIdAsync(createdProduct.Id));
    }
}
