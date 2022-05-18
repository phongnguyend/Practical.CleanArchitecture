using ClassifiedAds.Gateways.WebAPI.ConfigurationOptions;
using ClassifiedAds.Gateways.WebAPI.HttpMessageHandlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.Configuration.File;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassifiedAds.Gateways.Ocelot
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            AppSettings = new AppSettings();
            Configuration.Bind(AppSettings);
        }

        public IConfiguration Configuration { get; }

        private AppSettings AppSettings { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOcelot()
                .AddDelegatingHandler<DebuggingHandler>(true);

            services.PostConfigure<FileConfiguration>(configuration =>
            {
                foreach (var route in AppSettings.Ocelot.Routes.Select(x => x.Value))
                {
                    var uri = new Uri(route.Downstream);

                    foreach (var pathTemplate in route.UpstreamPathTemplates)
                    {

                        configuration.Routes.Add(new FileRoute
                        {
                            UpstreamPathTemplate = pathTemplate,
                            DownstreamPathTemplate = pathTemplate,
                            DownstreamScheme = uri.Scheme,
                            DownstreamHostAndPorts = new List<FileHostAndPort>
                            {
                                new FileHostAndPort{ Host = uri.Host, Port = uri.Port }
                            }
                        });
                    }
                }

                foreach (var route in configuration.Routes)
                {
                    if (string.IsNullOrWhiteSpace(route.DownstreamScheme))
                    {
                        route.DownstreamScheme = Configuration["Ocelot:DefaultDownstreamScheme"];
                    }

                    if (string.IsNullOrWhiteSpace(route.DownstreamPathTemplate))
                    {
                        route.DownstreamPathTemplate = route.UpstreamPathTemplate;
                    }
                }
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseWebSockets();
            app.UseOcelot().Wait();
        }
    }
}
