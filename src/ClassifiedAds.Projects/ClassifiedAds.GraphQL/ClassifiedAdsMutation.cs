using ClassifiedAds.Domain.Entities;
using ClassifiedAds.GraphQL.DownstreamServices;
using ClassifiedAds.GraphQL.Types;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;

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
                    var productService = GetProductService(configuration, httpContext);
                    var createdProduct = productService.CreateProduct(product).GetAwaiter().GetResult();
                    return createdProduct;
                });

            Field<BooleanGraphType>(
                "deleteProduct",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "id" }),
                resolve: context =>
                {
                    var id = context.GetArgument<Guid>("id");
                    var productService = GetProductService(configuration, httpContext);
                    productService.DeleteProduct(id).GetAwaiter().GetResult();
                    return true;

                });
        }

        public ProductService GetProductService(IConfiguration configuration, IHttpContextAccessor httpContext)
        {
            var accessToken = httpContext.HttpContext.Request.Headers["Authorization"].ToString();
            var client = new HttpClient();
            client.BaseAddress = new Uri(configuration["ResourceServer:Endpoint"]);
            client.Timeout = new TimeSpan(0, 0, 30);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", accessToken);
            return new ProductService(client);
        }
    }
}
