using ClassifiedAds.Infrastructure.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;
var configuration = builder.Configuration;

builder.WebHost.UseClassifiedAdsLogger(configuration =>
{
    return new LoggingOptions();
});

services.AddReverseProxy().LoadFromConfig(configuration.GetSection("Yarp"));

// Configure the HTTP request pipeline.
var app = builder.Build();

app.MapReverseProxy();

app.Run();
