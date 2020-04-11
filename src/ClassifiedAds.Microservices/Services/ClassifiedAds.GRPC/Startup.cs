using ClassifiedAds.Application.Events;
using ClassifiedAds.Domain.Identity;
using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using ClassifiedAds.Infrastructure.Identity;
using ClassifiedAds.Infrastructure.MessageBrokers.RabbitMQ;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace ClassifiedAds.GRPC
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();

            services.AddPersistence(Configuration["ConnectionStrings:ClassifiedAds"])
                .AddDomainServices()
                .AddMessageHandlers();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<ICurrentUser, CurrentWebUser>();

            services.AddSingleton<IMessageSender<FileUploadedEvent>>(new RabbitMQSender<FileUploadedEvent>(new RabbitMQSenderOptions
            {

            }));

            services.AddSingleton<IMessageSender<FileDeletedEvent>>(new RabbitMQSender<FileDeletedEvent>(new RabbitMQSenderOptions
            {

            }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints((Action<Microsoft.AspNetCore.Routing.IEndpointRouteBuilder>)(endpoints =>
            {
                GrpcEndpointRouteBuilderExtensions.MapGrpcService<ProductService>(endpoints);

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            }));
        }
    }
}
