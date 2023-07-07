using ClassifiedAds.CrossCuttingConcerns.HtmlGenerator;
using ClassifiedAds.CrossCuttingConcerns.PdfConverter;
using ClassifiedAds.Infrastructure.Web.MinimalApis;
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
using System.IO;
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

    private static async Task<IResult> HandleAsync(IMediator dispatcher,
        IHtmlGenerator htmlGenerator,
        IPdfConverter pdfConverter)
    {
        var products = await dispatcher.Send(new GetProductsQuery());
        var model = products.ToModels();

        var template = Path.Combine(Environment.CurrentDirectory, $"Templates/ProductList.cshtml");
        var html = await htmlGenerator.GenerateAsync(template, model);
        var pdf = await pdfConverter.ConvertAsync(html);

        return Results.File(pdf, MediaTypeNames.Application.Octet, "Products.pdf");
    }
}
