using ClassifiedAds.Application;
using ClassifiedAds.CrossCuttingConcerns.Csv;
using ClassifiedAds.Infrastructure.Web.MinimalApis;
using ClassifiedAds.Services.Product.Csv;
using ClassifiedAds.Services.Product.Queries;
using ClassifiedAds.Services.Product.RateLimiterPolicies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
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
        .WithTags("Products");
    }

    private static async Task<IResult> HandleAsync(Dispatcher dispatcher,
        ICsvWriter<ExportProductsToCsv> productCsvWriter)
    {
        var products = await dispatcher.DispatchAsync(new GetProductsQuery());
        using var stream = new MemoryStream();
        await productCsvWriter.WriteAsync(new ExportProductsToCsv { Products = products }, stream);
        return Results.File(stream.ToArray(), MediaTypeNames.Application.Octet, "Products.csv");
    }
}
