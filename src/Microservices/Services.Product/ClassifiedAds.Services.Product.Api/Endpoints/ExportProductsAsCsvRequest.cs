using ClassifiedAds.CrossCuttingConcerns.Csv;
using ClassifiedAds.Infrastructure.Web.MinimalApis;
using ClassifiedAds.Services.Product.Models;
using ClassifiedAds.Services.Product.Queries;
using ClassifiedAds.Services.Product.RateLimiterPolicies;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Product.Api.Endpoints;

public class ExportProductsAsCsvRequest : IEndpointHandler
{
    public static void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("api/products/exportascsv", HandleAsync)
        .RequireAuthorization()
        .RequireRateLimiting(RateLimiterPolicyNames.DefaultPolicy)
        .WithName("ExportProductsAsCsv")
        .Produces(StatusCodes.Status200OK)
        .WithOpenApi(operation => new OpenApiOperation(operation)
        {
            Tags = new List<OpenApiTag> { new OpenApiTag { Name = "Products" } }
        });
    }

    private static async Task<IResult> HandleAsync(IMediator dispatcher,
        ICsvWriter<ProductModel> productCsvWriter)
    {
        var products = await dispatcher.Send(new GetProductsQuery());
        var model = products.ToModels();
        using var stream = new MemoryStream();
        productCsvWriter.Write(model, stream);
        return Results.File(stream.ToArray(), MediaTypeNames.Application.Octet, "Products.csv");
    }
}
