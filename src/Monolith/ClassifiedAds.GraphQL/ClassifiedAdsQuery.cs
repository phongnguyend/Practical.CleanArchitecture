using ClassifiedAds.Domain.Entities;
using ClassifiedAds.GraphQL.DownstreamServices;
using GraphQL;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClassifiedAds.GraphQL;

public class ClassifiedAdsQuery
{
    public static Task<Product> Product([FromServices] IConfiguration configuration, [FromServices] IHttpContextAccessor httpContext, [Id] string id)
    {
        var productService = GetProductService(configuration, httpContext);
        return productService.GetProductById(Guid.Parse(id));
    }

    public static Task<List<Product>> Products([FromServices] IConfiguration configuration, [FromServices] IHttpContextAccessor httpContext)
    {
        var productService = GetProductService(configuration, httpContext);
        return productService.GetProducts();
    }

    private static ProductService GetProductService(IConfiguration configuration, IHttpContextAccessor httpContext)
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
