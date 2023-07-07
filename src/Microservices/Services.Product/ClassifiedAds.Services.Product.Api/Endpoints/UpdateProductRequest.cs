using ClassifiedAds.Infrastructure.Web.MinimalApis;
using ClassifiedAds.Services.Product.Authorization;
using ClassifiedAds.Services.Product.Commands;
using ClassifiedAds.Services.Product.Models;
using ClassifiedAds.Services.Product.Queries;
using ClassifiedAds.Services.Product.RateLimiterPolicies;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Product.Api.Endpoints;

public class UpdateProductRequest : IEndpointHandler
{
    public static void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPut("api/products/{id}", HandleAsync)
        .RequireAuthorization(AuthorizationPolicyNames.UpdateProductPolicy)
        .RequireRateLimiting(RateLimiterPolicyNames.DefaultPolicy)
        .WithName("UpdateProduct")
        .Produces<ProductModel>(StatusCodes.Status200OK, contentType: "application/json")
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithOpenApi(operation => new OpenApiOperation(operation)
        {
            Tags = new List<OpenApiTag> { new OpenApiTag { Name = "Products" } }
        });
    }

    private static async Task<IResult> HandleAsync(IMediator dispatcher, Guid id, [FromBody] ProductModel model)
    {
        var product = await dispatcher.Send(new GetProductQuery { Id = id, ThrowNotFoundIfNull = true });

        product.Code = model.Code;
        product.Name = model.Name;
        product.Description = model.Description;

        await dispatcher.Send(new AddUpdateProductCommand { Product = product });

        model = product.ToModel();

        return Results.Ok(model);
    }
}
