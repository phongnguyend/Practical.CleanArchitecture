using ClassifiedAds.Infrastructure.Web.MinimalApis;
using ClassifiedAds.Services.Product.Authorization;
using ClassifiedAds.Services.Product.Models;
using ClassifiedAds.Services.Product.Queries;
using ClassifiedAds.Services.Product.RateLimiterPolicies;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Product.Api.Endpoints;

public class GetProductsRequest : IEndpointHandler
{
    public static void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("api/products", HandleAsync)
        .RequireAuthorization(AuthorizationPolicyNames.GetProductsPolicy)
        .RequireRateLimiting(RateLimiterPolicyNames.DefaultPolicy)
        .WithName("GetProducts")
        .Produces<IEnumerable<ProductModel>>(contentType: "application/json")
        .WithOpenApi(operation => new OpenApiOperation(operation)
        {
            Tags = new List<OpenApiTag> { new OpenApiTag { Name = "Products" } }
        });
    }

    private static async Task<IResult> HandleAsync(IMediator dispatcher,
        ILogger<GetProductsRequest> logger)
    {
        logger.LogInformation("Getting all products");
        var products = await dispatcher.Send(new GetProductsQuery());
        var model = products.ToModels();
        return Results.Ok(model);
    }
}
