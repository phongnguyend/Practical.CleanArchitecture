using ClassifiedAds.CrossCuttingConcerns.Pdf;
using ClassifiedAds.Infrastructure.Web.MinimalApis;
using ClassifiedAds.Services.Product.Pdf;
using ClassifiedAds.Services.Product.Queries;
using ClassifiedAds.Services.Product.RateLimiterPolicies;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Product.Api.Endpoints;

public class ExportProductsAsPdfRequest : IEndpointHandler
{
    public static void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("api/products/exportaspdf", HandleAsync)
        .RequireAuthorization()
        .RequireRateLimiting(RateLimiterPolicyNames.DefaultPolicy)
        .WithName("ExportProductsAsPdf")
        .Produces(StatusCodes.Status200OK)
        .WithOpenApi(operation => new OpenApiOperation(operation)
        {
            Tags = new List<OpenApiTag> { new OpenApiTag { Name = "Products" } }
        });
    }

    private static async Task<IResult> HandleAsync(IMediator dispatcher, IPdfWriter<ExportProductsToPdf> pdfWriter)
    {
        var products = await dispatcher.Send(new GetProductsQuery());
        var bytes = await pdfWriter.GetBytesAsync(new ExportProductsToPdf { Products = products });

        return Results.File(bytes, MediaTypeNames.Application.Octet, "Products.pdf");
    }
}
