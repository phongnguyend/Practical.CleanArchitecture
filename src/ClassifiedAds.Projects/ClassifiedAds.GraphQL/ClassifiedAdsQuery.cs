using ClassifiedAds.GraphQL.DownstreamServices;
using ClassifiedAds.GraphQL.Types;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;

namespace ClassifiedAds.GraphQL
{
    public class ClassifiedAdsQuery : ObjectGraphType<object>
    {
        public ClassifiedAdsQuery(IConfiguration configuration, IHttpContextAccessor httpContext)
        {
            Name = "Query";

            Field<ListGraphType<ProductType>>(
                "products",
                resolve: context =>
                {
                    var productService = GetProductService(configuration, httpContext);
                    return productService.GetProducts();
                }
                );

            Field<ProductType>(
                "product",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "id" }),
                resolve: context =>
                {
                    var id = context.GetArgument<string>("id");
                    var productService = GetProductService(configuration, httpContext);
                    return productService.GetProductById(Guid.Parse(id));
                }
            );
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
