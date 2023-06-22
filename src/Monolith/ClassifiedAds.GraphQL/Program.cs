using ClassifiedAds.Infrastructure.Logging;
using GraphQL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ClassifiedAds.GraphQL;

public class Program
{
    public static void Main(string[] args)
    {
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

        // Configure the HTTP request pipeline.
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        // add http for Schema at default url /graphql
        app.UseGraphQL();

        // use graphql-playground at default url /ui/playground
        app.UseGraphQLPlayground();

        app.Run();
    }
}
