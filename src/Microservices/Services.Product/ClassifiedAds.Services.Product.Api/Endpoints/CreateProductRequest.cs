using ClassifiedAds.Infrastructure.Web.MinimalApis;
using ClassifiedAds.Services.Product.Authorization;
using ClassifiedAds.Services.Product.Commands;
using ClassifiedAds.Services.Product.Models;
using ClassifiedAds.Services.Product.RateLimiterPolicies;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Product.Api.Endpoints;

public class CreateProductRequest : IEndpointHandler
{
    public static void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost("api/products", HandleAsync)
        .RequireAuthorization(AuthorizationPolicyNames.AddProductPolicy)
        .RequireRateLimiting(RateLimiterPolicyNames.DefaultPolicy)
        .WithName("CreateProduct")
        .Produces<ProductModel>(StatusCodes.Status201Created, contentType: "application/json")
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithOpenApi(operation => new OpenApiOperation(operation)
        {
            Tags = new List<OpenApiTag> { new OpenApiTag { Name = "Products" } }
        });
    }

    private static async Task<IResult> HandleAsync(IMediator dispatcher, ProductModel model)
    {
        var product = model.ToEntity();
        await dispatcher.Send(new AddUpdateProductCommand { Product = product });
        model = product.ToModel();
        return Results.Created($"/api/products/{model.Id}", model);
    }
}
