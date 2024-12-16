using ClassifiedAds.GraphQL;
using ClassifiedAds.Infrastructure.HealthChecks;
using ClassifiedAds.Infrastructure.HostedServices;
using ClassifiedAds.Infrastructure.Logging;
using GraphQL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;
var configuration = builder.Configuration;

builder.WebHost.UseClassifiedAdsLogger(configuration =>
        {
            return new LoggingOptions();
        });

// If using Kestrel:
services.Configure<KestrelServerOptions>(options =>
{
    options.AllowSynchronousIO = true;
});

// If using IIS:
services.Configure<IISServerOptions>(options =>
{
    options.AllowSynchronousIO = true;
});

services.AddLogging(builder => builder.AddConsole());
services.AddHttpContextAccessor();

services.AddGraphQL(b => b.AddAutoSchema<ClassifiedAdsQuery>(s => s.WithMutation<ClassifiedAdsMutation>())
                    .AddSystemTextJson());

services.AddHealthChecks()
    .AddHttp(configuration["ResourceServer:Endpoint"], name: "Resource (Web API) Server", failureStatus: HealthStatus.Degraded);

services.Configure<HealthChecksBackgroundServiceOptions>(x => x.Interval = TimeSpan.FromMinutes(10));
services.AddHostedService<HealthChecksBackgroundService>();

// Configure the HTTP request pipeline.
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHealthChecks("/healthz", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = HealthChecksResponseWriter.WriteReponse,
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status500InternalServerError,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable,
    },
});

// add http for Schema at default url /graphql
app.UseGraphQL();

// use graphql-playground at default url /ui/playground
app.UseGraphQLPlayground();

app.Run();
