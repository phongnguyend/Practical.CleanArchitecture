using ClassifiedAds.Infrastructure.Web.MinimalApis;
using ClassifiedAds.Services.Product.Authorization;
using ClassifiedAds.Services.Product.Commands;
using ClassifiedAds.Services.Product.Queries;
using ClassifiedAds.Services.Product.RateLimiterPolicies;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Product.Api.Endpoints;

public class UpdateProductRequest
{
    public string Code { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public class Validator : AbstractValidator<UpdateProductRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Code).NotEmpty();
        }
    }
}

public class UpdateProductResponse
{
    public Guid Id { get; set; }

    public string Code { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }
}

public class UpdateProductRequestHandler : IEndpointHandler
{
    public static void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPut("api/products/{id}", HandleAsync)
        .RequireAuthorization(AuthorizationPolicyNames.UpdateProductPolicy)
        .RequireRateLimiting(RateLimiterPolicyNames.DefaultPolicy)
        .WithName("UpdateProduct")
        .Produces<UpdateProductResponse>(StatusCodes.Status200OK, contentType: "application/json")
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithOpenApi(operation => new OpenApiOperation(operation)
        {
            Tags = new List<OpenApiTag> { new OpenApiTag { Name = "Products" } }
        });
    }

    private static async Task<IResult> HandleAsync(IMediator dispatcher, Guid id, [FromBody] UpdateProductRequest request, IValidator<UpdateProductRequest> validator)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary(),
                statusCode: (int)HttpStatusCode.BadRequest);
        }

        var product = await dispatcher.Send(new GetProductQuery { Id = id, ThrowNotFoundIfNull = true });

        product.Code = request.Code;
        product.Name = request.Name;
        product.Description = request.Description;

        await dispatcher.Send(new AddUpdateProductCommand { Product = product });

        var response = new UpdateProductResponse
        {
            Id = product.Id,
            Code = product.Code,
            Name = product.Name,
            Description = product.Description,
        };

        return Results.Ok(response);
    }
}
