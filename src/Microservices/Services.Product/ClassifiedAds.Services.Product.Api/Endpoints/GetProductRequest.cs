using ClassifiedAds.Infrastructure.Web.MinimalApis;
using ClassifiedAds.Services.Product.Authorization;
using ClassifiedAds.Services.Product.Models;
using ClassifiedAds.Services.Product.Queries;
using ClassifiedAds.Services.Product.RateLimiterPolicies;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Product.Api.Endpoints;

public class GetProductRequest : IEndpointHandler
{
    public static void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("api/products/{id}", HandleAsync)
        .RequireAuthorization(AuthorizationPolicyNames.GetProductPolicy)
        .RequireRateLimiting(RateLimiterPolicyNames.DefaultPolicy)
        .WithName("GetProduct")
        .Produces<ProductModel>(contentType: "application/json")
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithOpenApi(operation => new OpenApiOperation(operation)
        {
            Tags = new List<OpenApiTag> { new OpenApiTag { Name = "Products" } }
        });
    }

    private static async Task<IResult> HandleAsync(IMediator dispatcher, Guid id)
    {
        var product = await dispatcher.Send(new GetProductQuery { Id = id, ThrowNotFoundIfNull = true });
        var model = product.ToModel();
        return Results.Ok(model);
    }
}
