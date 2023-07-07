using ClassifiedAds.Infrastructure.Web.MinimalApis;
using ClassifiedAds.Services.Product.Authorization;
using ClassifiedAds.Services.Product.Commands;
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

public class DeleteProductRequest : IEndpointHandler
{
    public static void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapDelete("api/products/{id:guid}", HandleAsync)
        .RequireAuthorization(AuthorizationPolicyNames.DeleteProductPolicy)
        .RequireRateLimiting(RateLimiterPolicyNames.DefaultPolicy)
        .WithName("DeleteProduct")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithOpenApi(operation => new OpenApiOperation(operation)
        {
            Tags = new List<OpenApiTag> { new OpenApiTag { Name = "Products" } }
        });
    }

    private static async Task<IResult> HandleAsync(IMediator dispatcher, Guid id)
    {
        var product = await dispatcher.Send(new GetProductQuery { Id = id, ThrowNotFoundIfNull = true });

        await dispatcher.Send(new DeleteProductCommand { Product = product });

        return Results.Ok();
    }
}
