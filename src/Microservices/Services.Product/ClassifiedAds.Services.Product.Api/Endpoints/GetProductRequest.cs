using ClassifiedAds.Application;
using ClassifiedAds.Infrastructure.Web.MinimalApis;
using ClassifiedAds.Services.Product.Authorization;
using ClassifiedAds.Services.Product.Models;
using ClassifiedAds.Services.Product.Queries;
using ClassifiedAds.Services.Product.RateLimiterPolicies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Product.Api.Endpoints;

public class GetProductRequest : IEndpointHandler
{
    public static void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("api/products/{id}", HandleAsync)
        .RequireAuthorization(Permissions.GetProduct)
        .RequireRateLimiting(RateLimiterPolicyNames.DefaultPolicy)
        .WithName("GetProduct")
        .Produces<ProductModel>(contentType: "application/json")
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithTags("Products");
    }

    private static async Task<IResult> HandleAsync(Dispatcher dispatcher, Guid id)
    {
        var product = await dispatcher.DispatchAsync(new GetProductQuery { Id = id, ThrowNotFoundIfNull = true });
        var model = product.ToModel();
        return Results.Ok(model);
    }
}
