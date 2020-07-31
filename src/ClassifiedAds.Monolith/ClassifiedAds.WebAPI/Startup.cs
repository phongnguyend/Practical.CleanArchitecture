using AutoMapper;
using ClassifiedAds.Application.FileEntries.DTOs;
using ClassifiedAds.Domain.Identity;
using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using ClassifiedAds.Domain.Infrastructure.Storages;
using ClassifiedAds.Infrastructure.Identity;
using ClassifiedAds.Infrastructure.MessageBrokers.AzureEventGrid;
using ClassifiedAds.Infrastructure.MessageBrokers.AzureEventHub;
using ClassifiedAds.Infrastructure.MessageBrokers.AzureQueue;
using ClassifiedAds.Infrastructure.MessageBrokers.AzureServiceBus;
using ClassifiedAds.Infrastructure.MessageBrokers.Kafka;
using ClassifiedAds.Infrastructure.MessageBrokers.RabbitMQ;
using ClassifiedAds.Infrastructure.Storages.Amazon;
using ClassifiedAds.Infrastructure.Storages.Azure;
using ClassifiedAds.Infrastructure.Storages.Local;
using ClassifiedAds.Infrastructure.Web.Filters;
using ClassifiedAds.WebAPI.ConfigurationOptions;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;

namespace ClassifiedAds.WebAPI
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
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Startup));

            services.Configure<AppSettings>(Configuration);

            services.AddClassifiedAdsMonitoringServices();

            services.AddControllers(configure =>
            {
                configure.Filters.Add(typeof(GlobalExceptionFilter));
            }).AddClassifiedAdsMonitoringServices();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowedOrigins", builder => builder
                    .WithOrigins(AppSettings.CORS.AllowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader());

                options.AddPolicy("AllowAnyOrigin", builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());

                options.AddPolicy("CustomPolicy", builder => builder
                    .AllowAnyOrigin()
                    .WithMethods("Get")
                    .WithHeaders("Content-Type"));
            });

            services.AddPersistence(AppSettings.ConnectionStrings.ClassifiedAds)
                    .AddDomainServices()
                    .AddApplicationServices((Type serviceType, Type implementationType, ServiceLifetime serviceLifetime) =>
                    {
                        services.AddInterceptors(serviceType, implementationType, serviceLifetime, AppSettings.Interceptors);
                    })
                    .AddMessageHandlers()
                    .ConfigureInterceptors()
                    .AddIdentityCore();

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = AppSettings.IdentityServerAuthentication.Authority;
                    options.ApiName = AppSettings.IdentityServerAuthentication.ApiName;
                    options.RequireHttpsMetadata = AppSettings.IdentityServerAuthentication.RequireHttpsMetadata;
                });

            services.AddSwaggerGen(setupAction =>
            {
                setupAction.SwaggerDoc(
                    $"ClassifiedAds",
                    new OpenApiInfo()
                    {
                        Title = "ClassifiedAds API",
                        Version = "1",
                        Description = "ClassifiedAds API Specification.",
                        Contact = new OpenApiContact
                        {
                            Email = "abc.xyz@gmail.com",
                            Name = "Phong Nguyen",
                            Url = new Uri("https://github.com/phongnguyend"),
                        },
                        License = new OpenApiLicense
                        {
                            Name = "MIT License",
                            Url = new Uri("https://opensource.org/licenses/MIT"),
                        },
                    });

                setupAction.AddSecurityDefinition("bearer", new OpenApiSecurityScheme()
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "Input your Bearer token to access this API",
                });

                setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "bearer",
                            },
                        }, new List<string>()
                    },
                });
            });

            services.AddCaches(AppSettings.Caching)
                    .AddClassifiedAdsProfiler(AppSettings.Monitoring);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<ICurrentUser, CurrentWebUser>();

            if (AppSettings.Storage.UsedAzure())
            {
                services.AddSingleton<IFileStorageManager>(new AzureBlobStorageManager(AppSettings.Storage.Azure.ConnectionString, AppSettings.Storage.Azure.Container));
            }
            else if (AppSettings.Storage.UsedAmazon())
            {
                services.AddSingleton<IFileStorageManager>(
                    new AmazonS3StorageManager(
                        AppSettings.Storage.Amazon.AccessKeyID,
                        AppSettings.Storage.Amazon.SecretAccessKey,
                        AppSettings.Storage.Amazon.BucketName,
                        AppSettings.Storage.Amazon.RegionEndpoint));
            }
            else
            {
                services.AddSingleton<IFileStorageManager>(new LocalFileStorageManager(AppSettings.Storage.Local.Path));
            }

            if (AppSettings.MessageBroker.UsedRabbitMQ())
            {
                services.AddSingleton<IMessageSender<FileUploadedEvent>>(new RabbitMQSender<FileUploadedEvent>(new RabbitMQSenderOptions
                {
                    HostName = AppSettings.MessageBroker.RabbitMQ.HostName,
                    UserName = AppSettings.MessageBroker.RabbitMQ.UserName,
                    Password = AppSettings.MessageBroker.RabbitMQ.Password,
                    ExchangeName = AppSettings.MessageBroker.RabbitMQ.ExchangeName,
                    RoutingKey = AppSettings.MessageBroker.RabbitMQ.RoutingKey_FileUploaded,
                }));

                services.AddSingleton<IMessageSender<FileDeletedEvent>>(new RabbitMQSender<FileDeletedEvent>(new RabbitMQSenderOptions
                {
                    HostName = AppSettings.MessageBroker.RabbitMQ.HostName,
                    UserName = AppSettings.MessageBroker.RabbitMQ.UserName,
                    Password = AppSettings.MessageBroker.RabbitMQ.Password,
                    ExchangeName = AppSettings.MessageBroker.RabbitMQ.ExchangeName,
                    RoutingKey = AppSettings.MessageBroker.RabbitMQ.RoutingKey_FileDeleted,
                }));
            }
            else if (AppSettings.MessageBroker.UsedKafka())
            {
                services.AddSingleton<IMessageSender<FileUploadedEvent>>(new KafkaSender<FileUploadedEvent>(
                    AppSettings.MessageBroker.Kafka.BootstrapServers,
                    AppSettings.MessageBroker.Kafka.Topic_FileUploaded));

                services.AddSingleton<IMessageSender<FileDeletedEvent>>(new KafkaSender<FileDeletedEvent>(
                    AppSettings.MessageBroker.Kafka.BootstrapServers,
                    AppSettings.MessageBroker.Kafka.Topic_FileDeleted));
            }
            else if (AppSettings.MessageBroker.UsedAzureQueue())
            {
                services.AddSingleton<IMessageSender<FileUploadedEvent>>(new AzureQueueSender<FileUploadedEvent>(
                    connectionString: AppSettings.MessageBroker.AzureQueue.ConnectionString,
                    queueName: AppSettings.MessageBroker.AzureQueue.QueueName_FileUploaded));

                services.AddSingleton<IMessageSender<FileDeletedEvent>>(new AzureQueueSender<FileDeletedEvent>(
                    connectionString: AppSettings.MessageBroker.AzureQueue.ConnectionString,
                    queueName: AppSettings.MessageBroker.AzureQueue.QueueName_FileDeleted));
            }
            else if (AppSettings.MessageBroker.UsedAzureServiceBus())
            {
                services.AddSingleton<IMessageSender<FileUploadedEvent>>(new AzureServiceBusSender<FileUploadedEvent>(
                    connectionString: AppSettings.MessageBroker.AzureServiceBus.ConnectionString,
                    queueName: AppSettings.MessageBroker.AzureServiceBus.QueueName_FileUploaded));

                services.AddSingleton<IMessageSender<FileDeletedEvent>>(new AzureServiceBusSender<FileDeletedEvent>(
                    connectionString: AppSettings.MessageBroker.AzureServiceBus.ConnectionString,
                    queueName: AppSettings.MessageBroker.AzureServiceBus.QueueName_FileDeleted));
            }
            else if (AppSettings.MessageBroker.UsedAzureEventGrid())
            {
                services.AddSingleton<IMessageSender<FileUploadedEvent>>(new AzureEventGridSender<FileUploadedEvent>(
                                AppSettings.MessageBroker.AzureEventGrid.DomainEndpoint,
                                AppSettings.MessageBroker.AzureEventGrid.DomainKey,
                                AppSettings.MessageBroker.AzureEventGrid.Topic_FileUploaded));

                services.AddSingleton<IMessageSender<FileDeletedEvent>>(new AzureEventGridSender<FileDeletedEvent>(
                                AppSettings.MessageBroker.AzureEventGrid.DomainEndpoint,
                                AppSettings.MessageBroker.AzureEventGrid.DomainKey,
                                AppSettings.MessageBroker.AzureEventGrid.Topic_FileDeleted));
            }
            else if (AppSettings.MessageBroker.UsedAzureEventHub())
            {
                services.AddSingleton<IMessageSender<FileUploadedEvent>>(new AzureEventHubSender<FileUploadedEvent>(
                                AppSettings.MessageBroker.AzureEventHub.ConnectionString,
                                AppSettings.MessageBroker.AzureEventHub.Hub_FileUploaded));

                services.AddSingleton<IMessageSender<FileDeletedEvent>>(new AzureEventHubSender<FileDeletedEvent>(
                                AppSettings.MessageBroker.AzureEventHub.ConnectionString,
                                AppSettings.MessageBroker.AzureEventHub.Hub_FileDeleted));
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDebuggingMiddleware();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseClassifiedAdsMonitoringServices();

            app.UseRouting();

            app.UseCors(AppSettings.CORS.AllowAnyOrigin ? "AllowAnyOrigin" : "AllowedOrigins");

            app.UseSecurityHeaders(AppSettings.SecurityHeaders);

            app.UseSwagger();

            app.UseSwaggerUI(setupAction =>
            {
                setupAction.SwaggerEndpoint(
                    "/swagger/ClassifiedAds/swagger.json",
                    "ClassifiedAds API");

                setupAction.RoutePrefix = string.Empty;

                setupAction.DefaultModelExpandDepth(2);
                setupAction.DefaultModelRendering(ModelRendering.Model);
                setupAction.DocExpansion(DocExpansion.None);
                setupAction.EnableDeepLinking();
                setupAction.DisplayOperationId();
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseClassifiedAdsProfiler();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
