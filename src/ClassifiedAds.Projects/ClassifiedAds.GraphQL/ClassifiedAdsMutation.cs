using ClassifiedAds.CrossCuttingConcerns.ExtensionMethods;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.GraphQL.Types;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ClassifiedAds.GraphQL
{
    public class ClassifiedAdsMutation : ObjectGraphType
    {
        public ClassifiedAdsMutation(IConfiguration configuration, IHttpContextAccessor httpContext)
        {
            Name = "Mutation";

            Field<ProductType>(
                "createProduct",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<ProductInputType>> { Name = "product" }
                ),
                resolve: context =>
                {
                    var product = context.GetArgument<Product>("product");
                    var httpClient = GetHttpClient(configuration, httpContext);
                    var createdProduct = CreateProduct(product, httpClient).GetAwaiter().GetResult();
                    return createdProduct;
                });

            Field<BooleanGraphType>(
                "deleteProduct",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "id" }),
                resolve: context =>
                {
                    var id = context.GetArgument<Guid>("id");
                    var httpClient = GetHttpClient(configuration, httpContext);
                    DeleteProduct(id, httpClient).GetAwaiter().GetResult();
                    return true;

                });
        }

        public HttpClient GetHttpClient(IConfiguration configuration, IHttpContextAccessor httpContext)
        {
            var accessToken = httpContext.HttpContext.Request.Headers["Authorization"].ToString();

            var client = new HttpClient();
            client.BaseAddress = new Uri(configuration["ResourceServer:Endpoint"]);
            client.Timeout = new TimeSpan(0, 0, 30);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", accessToken);
            return client;
        }

        private async Task<Product> CreateProduct(Product product, HttpClient httpClient)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/products");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            request.Content = product.AsJsonContent();

            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var createdProduct = await response.Content.ReadAs<Product>();
            return createdProduct;
        }

        private async Task DeleteProduct(Guid id, HttpClient httpClient)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"api/products/{id}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
        }
    }
}
