using ClassifiedAds.Infrastructure.Web.MinimalApis;
using ClassifiedAds.Services.Product.Authorization;
using ClassifiedAds.Services.Product.Commands;
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

public class CreateProductRequest
{
    public string Code { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public class Validator : AbstractValidator<CreateProductRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Code).NotEmpty();
        }
    }
}

public class CreateProductResponse
{
    public Guid Id { get; set; }

    public string Code { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }
}

public class CreateProductRequestHandler : IEndpointHandler
{
    public static void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost("api/products", HandleAsync)
        .RequireAuthorization(AuthorizationPolicyNames.AddProductPolicy)
        .RequireRateLimiting(RateLimiterPolicyNames.DefaultPolicy)
        .WithName("CreateProduct")
        .Produces<CreateProductResponse>(StatusCodes.Status201Created, contentType: "application/json")
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithOpenApi(operation => new OpenApiOperation(operation)
        {
            Tags = new List<OpenApiTag> { new OpenApiTag { Name = "Products" } }
        });
    }

    private static async Task<IResult> HandleAsync(IMediator dispatcher, [FromBody] CreateProductRequest request, IValidator<CreateProductRequest> validator)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary(),
                statusCode: (int)HttpStatusCode.BadRequest);
        }

        var product = new Entities.Product
        {
            Code = request.Code,
            Name = request.Name,
            Description = request.Description,
        };

        await dispatcher.Send(new AddUpdateProductCommand { Product = product });

        var response = new CreateProductResponse
        {
            Id = product.Id,
            Code = product.Code,
            Name = product.Name,
            Description = product.Description,
        };

        return Results.Created($"/api/products/{response.Id}", response);
    }
}
