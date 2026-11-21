using System.Net;
using System.Text.Json;
using ClassifiedAds.Mobile.Core.Models;
using ClassifiedAds.Mobile.Core.Network;
using Xunit;

namespace ClassifiedAds.Mobile.Tests;

public class WorkflowTests
{
    [Fact]
    public void ProductValidationMatchesTheMobileContract()
    {
        var product = new Product();
        Assert.Equal("Name is required.", product.Validate());
        product.Name = "Phone";
        product.Code = "12345678901";
        Assert.Contains("10 characters", product.Validate());
        product.Code = "PHONE";
        product.Description = "Test phone";
        Assert.Null(product.Validate());
    }

    [Fact]
    public void UserRejectsInvalidEmailAndNegativeAttempts()
    {
        var user = new User
        {
            UserName = "test",
            Email = "invalid"
        };
        Assert.Equal("Enter a valid email.", user.Validate());
        user.Email = "test@example.com";
        user.AccessFailedCount = -1;
        Assert.Contains("nonnegative", user.Validate());
    }

    [Fact]
    public void RequiredFieldsApplyToEveryEditor()
    {
        Assert.Equal("Name is required.", new Role().Validate());
        Assert.Equal("Name is required.", new StoredFile().Validate());
        Assert.Equal("Username is required.", new User().Validate());
        Assert.Equal("Key is required.", new ConfigurationEntry().Validate());
        Assert.Equal("Value is required.", new ConfigurationEntry { Key = "test" }.Validate());
    }

    [Fact]
    public void PaginationClampsAfterFilteringAndDeleting()
    {
        var pages = new Pagination();
        pages.SetTotal(59);
        pages.Next();
        pages.Next();
        pages.Next();
        Assert.Equal("Page 4 \u00b7 59 records", pages.Summary);
        Assert.Equal(new[] { 15, 16, 17, 18, 19 }, pages.Slice(Enumerable.Range(0, 59)));
        pages.SetTotal(0);
        Assert.Equal(1, pages.Page);
        Assert.False(pages.HasNext);
        Assert.False(pages.HasPrevious);
        pages.Next();
        pages.Previous();
        Assert.Equal(1, pages.Page);
    }

    [Fact]
    public async Task ProductCreateUpdateHistoryDeleteUseExactRoutesAndPayloads()
    {
        var handler = new RecordingHandler();
        var repository = new Repository<Product>(Client(handler), "products");
        var product = new Product
        {
            Name = "Phone",
            Code = "P1",
            Description = "Test",
            Price = 99
        };
        await repository.Save(product);
        Assert.Equal("POST", handler.Method);
        Assert.Equal("/api/products", handler.Path);
        using (var json = JsonDocument.Parse(handler.Body!))
        {
            Assert.False(json.RootElement.TryGetProperty("id", out _));
            Assert.False(json.RootElement.TryGetProperty("price", out _));
        }

        product.Id = "abc";
        await repository.Save(product);
        Assert.Equal("PUT", handler.Method);
        Assert.Equal("/api/products/abc", handler.Path);
        handler.Response = "[]";
        await repository.History("abc");
        Assert.Equal("/api/products/abc/auditlogs", handler.Path);
        await repository.Delete("abc");
        Assert.Equal("DELETE", handler.Method);
        Assert.Equal("Bearer token", handler.Authorization);
    }

    [Theory]
    [InlineData("users")]
    [InlineData("roles")]
    [InlineData("files")]
    [InlineData("configurationentries")]
    public async Task ResourceDeleteEncodesIdentifiers(string resource)
    {
        var handler = new RecordingHandler();
        await Client(handler).Write(ApiClient.PathFor(resource, "a/b"), HttpMethod.Delete);
        Assert.EndsWith("/a%2Fb", handler.Path, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task UploadSendsFileAndEncryptionMetadata()
    {
        var handler = new RecordingHandler();
        await Client(handler).Upload("files", new MemoryStream([1, 2, 3]), "report.txt", new Dictionary<string, string> { ["encrypted"] = "true", ["name"] = "Report" });
        Assert.Equal("POST", handler.Method);
        Assert.Contains("formFile", handler.Body);
        Assert.Contains("report.txt", handler.Body);
        Assert.Contains("encrypted", handler.Body);
        Assert.Contains("true", handler.Body);
    }

    [Fact]
    public async Task FailedRequestsPreserveStatusAndServerMessage()
    {
        var handler = new RecordingHandler
        {
            Status = HttpStatusCode.Forbidden,
            Response = "permission denied"
        };
        var error = await Assert.ThrowsAsync<HttpRequestException>(() => Client(handler).Get<object>("products"));
        Assert.Contains("403", error.Message);
        Assert.Contains("permission denied", error.Message);
    }

    private static ApiClient Client(RecordingHandler handler) => new(new HttpClient(handler) { BaseAddress = new Uri("https://example.test/api/") }, () => Task.FromResult("token"));
    private sealed class RecordingHandler : HttpMessageHandler
    {
        public string? Path, Method, Body, Authorization;
        public string Response = "{}";
        public HttpStatusCode Status = HttpStatusCode.OK;
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
        {
            Path = request.RequestUri!.AbsolutePath;
            Method = request.Method.Method;
            Authorization = request.Headers.Authorization?.ToString();
            Body = request.Content is null ? null : await request.Content.ReadAsStringAsync(token);
            return new HttpResponseMessage(Status)
            {
                Content = new StringContent(Response)
            };
        }
    }
}
