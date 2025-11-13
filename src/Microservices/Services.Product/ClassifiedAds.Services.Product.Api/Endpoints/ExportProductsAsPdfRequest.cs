using ClassifiedAds.Application;
using ClassifiedAds.CrossCuttingConcerns.Pdf;
using ClassifiedAds.Infrastructure.Web.MinimalApis;
using ClassifiedAds.Services.Product.Pdf;
using ClassifiedAds.Services.Product.Queries;
using ClassifiedAds.Services.Product.RateLimiterPolicies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
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
        .WithTags("Products");
    }

    private static async Task<IResult> HandleAsync(Dispatcher dispatcher, IPdfWriter<ExportProductsToPdf> pdfWriter)
    {
        var products = await dispatcher.DispatchAsync(new GetProductsQuery());
        var bytes = await pdfWriter.GetBytesAsync(new ExportProductsToPdf { Products = products });

        return Results.File(bytes, MediaTypeNames.Application.Octet, "Products.pdf");
    }
}
