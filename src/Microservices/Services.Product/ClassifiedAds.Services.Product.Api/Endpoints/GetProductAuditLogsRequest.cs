using ClassifiedAds.Infrastructure.Web.MinimalApis;
using ClassifiedAds.Services.Product.Authorization;
using ClassifiedAds.Services.Product.DTOs;
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
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Product.Api.Endpoints;

public class GetProductAuditLogsRequest : IEndpointHandler
{
    public static void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("api/products/{id}/auditlogs", HandleAsync)
        .RequireAuthorization(AuthorizationPolicyNames.GetProductAuditLogsPolicy)
        .RequireRateLimiting(RateLimiterPolicyNames.DefaultPolicy)
        .WithName("GetProductAuditLogs")
        .Produces<IEnumerable<AuditLogEntryDTO>>(contentType: "application/json")
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithOpenApi(operation => new OpenApiOperation(operation)
        {
            Tags = new List<OpenApiTag> { new OpenApiTag { Name = "Products" } }
        });
    }

    private static async Task<IResult> HandleAsync(IMediator dispatcher, Guid id)
    {
        var logs = await dispatcher.Send(new GetAuditEntriesQuery { ObjectId = id.ToString() });

        List<dynamic> entries = new List<dynamic>();
        ProductModel previous = null;
        foreach (var log in logs.OrderBy(x => x.CreatedDateTime))
        {
            var data = JsonSerializer.Deserialize<ProductModel>(log.Log);
            var highLight = new
            {
                Code = previous != null && data.Code != previous.Code,
                Name = previous != null && data.Name != previous.Name,
                Description = previous != null && data.Description != previous.Description,
            };

            var entry = new
            {
                log.Id,
                log.UserName,
                Action = log.Action.Replace("_PRODUCT", string.Empty),
                log.CreatedDateTime,
                data,
                highLight,
            };
            entries.Add(entry);

            previous = data;
        }

        return Results.Ok(entries.OrderByDescending(x => x.CreatedDateTime));
    }
}
