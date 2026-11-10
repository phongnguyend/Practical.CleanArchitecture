using ClassifiedAds.Contracts.AuditLog.DTOs;
using ClassifiedAds.Application;
using ClassifiedAds.Contracts.AuditLog.Services;
using ClassifiedAds.Modules.Identity.Authorization;
using ClassifiedAds.Modules.Identity.Endpoints;
using ClassifiedAds.Modules.Identity.Queries;
using ClassifiedAds.Modules.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace ClassifiedAds.Modules.Identity.UnitTests;

public class UserAuditTests
{
    [Fact]
    public async Task HistoryOrdersSnapshotsHighlightsChangesAndOmitsSecrets()
    {
        var id = Guid.NewGuid();
        var service = new AuditService
        {
            Logs = new List<AuditLogEntryDTO>
            {
                Entry(id, "UPDATED_USER", 2, "new@example.com"),
                Entry(Guid.NewGuid(), "UPDATED_USER", 3, "other@example.com"),
                Entry(id, "CREATED_USER", 1, "old@example.com"),
                Entry(id, "GET_USERS", 4, "ignored@example.com"),
            },
        };
        var entries = await new GetUserAuditLogsQueryHandler(service).HandleAsync(new GetUserAuditLogsQuery { Id = id });
        Assert.Equal(id.ToString(), service.Options.ObjectId);
        Assert.True(service.Options.AsNoTracking);
        Assert.Equal(2, entries.Count);
        Assert.Equal("UPDATED", entries[0].Action);
        Assert.Equal("new@example.com", entries[0].Data.Email);
        Assert.True(entries[0].HighLight["email"]);
        Assert.False(entries[0].HighLight["userName"]);
        Assert.All(entries[1].HighLight.Values, value => Assert.False(value));
        var json = JsonSerializer.Serialize(entries);
        Assert.DoesNotContain("secret", json);
        Assert.DoesNotContain("Password", json);
        Assert.DoesNotContain("SecurityStamp", json);
    }

    [Fact]
    public async Task UserWithoutHistoryReturnsEmptyList()
    {
        var entries = await new GetUserAuditLogsQueryHandler(new AuditService())
            .HandleAsync(new GetUserAuditLogsQuery { Id = Guid.NewGuid() });
        Assert.Empty(entries);
    }

    [Fact]
    public async Task UserHistoryRouteIsRegisteredAndAuthorized()
    {
        var builder = WebApplication.CreateBuilder();
        await using var app = builder.Build();
        UsersEndpoints.Map(app);
        var endpoint = ((IEndpointRouteBuilder)app).DataSources.SelectMany(source => source.Endpoints)
            .OfType<RouteEndpoint>().Single(route => route.RoutePattern.RawText == "api/users/{id:guid}/auditlogs");
        Assert.Contains(endpoint.Metadata.GetOrderedMetadata<IAuthorizeData>(), data => data.Policy == Permissions.GetUser);
    }

    [Fact]
    public async Task UserHistoryRequestReturns200InsteadOfMissingRoute404()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseUrls("http://127.0.0.1:0");
        builder.Services.AddAuthorization(options => options.AddPolicy(Permissions.GetUser, policy => policy.RequireAssertion(_ => true)));
        builder.Services.AddSingleton<IAuditLogService>(new AuditService());
        builder.Services.AddScoped<Dispatcher>();
        builder.Services.AddScoped<IQueryHandler<GetUserAuditLogsQuery, List<UserAuditEntryModel>>, GetUserAuditLogsQueryHandler>();
        await using var app = builder.Build();
        app.UseAuthorization();
        UsersEndpoints.Map(app);
        await app.StartAsync();
        using var client = new HttpClient { BaseAddress = new Uri(app.Urls.Single()) };
        using var response = await client.GetAsync($"/api/users/{Guid.NewGuid()}/auditlogs");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("[]", await response.Content.ReadAsStringAsync());
    }

    private static AuditLogEntryDTO Entry(Guid userId, string action, int day, string email) => new AuditLogEntryDTO
    {
        Id = Guid.NewGuid(), ObjectId = userId.ToString(), Action = action,
        CreatedDateTime = new DateTimeOffset(2026, 1, day, 0, 0, 0, TimeSpan.Zero),
        UserName = "administrator",
        Log = JsonSerializer.Serialize(new { Id = userId, UserName = "test", Email = email, Password = "secret", PasswordHash = "secret", SecurityStamp = "secret" }),
    };

    private sealed class AuditService : IAuditLogService
    {
        public List<AuditLogEntryDTO> Logs { get; set; } = new List<AuditLogEntryDTO>();
        public AuditLogEntryQueryOptions Options { get; private set; }
        public Task AddAsync(AuditLogEntryDTO auditLog, string requestId) => throw new NotSupportedException();
        public Task<List<AuditLogEntryDTO>> GetAuditLogEntriesAsync(AuditLogEntryQueryOptions query)
        {
            Options = query;
            return Task.FromResult(Logs);
        }
    }
}
