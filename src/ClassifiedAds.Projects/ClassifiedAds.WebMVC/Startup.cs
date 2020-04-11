using Amazon;
using Amazon.S3;
using ClassifiedAds.Application.Events;
using ClassifiedAds.Domain.Identity;
using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using ClassifiedAds.Domain.Infrastructure.Storages;
using ClassifiedAds.Domain.Notification;
using ClassifiedAds.Infrastructure.Identity;
using ClassifiedAds.Infrastructure.Logging;
using ClassifiedAds.Infrastructure.MessageBrokers.AzureQueue;
using ClassifiedAds.Infrastructure.MessageBrokers.AzureServiceBus;
using ClassifiedAds.Infrastructure.MessageBrokers.Kafka;
using ClassifiedAds.Infrastructure.MessageBrokers.RabbitMQ;
using ClassifiedAds.Infrastructure.Notification;
using ClassifiedAds.Infrastructure.Storages.Amazon;
using ClassifiedAds.Infrastructure.Storages.Azure;
using ClassifiedAds.Infrastructure.Storages.Local;
using ClassifiedAds.WebMVC.Authorization;
using ClassifiedAds.WebMVC.ClaimsTransformations;
using ClassifiedAds.WebMVC.ConfigurationOptions;
using ClassifiedAds.WebMVC.Filters;
using ClassifiedAds.WebMVC.HttpHandlers;
using ClassifiedAds.WebMVC.Middleware;
using Hangfire;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;

namespace ClassifiedAds.WebMVC
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;

            AppSettings = new AppSettings();
            Configuration.Bind(AppSettings);

            var validationResult = AppSettings.Validate();
            if (validationResult.Failed)
            {
                throw new Exception(validationResult.FailureMessage);
            }

            Logger.Configure(env, AppSettings.LoggerOptions);
        }

        public IConfiguration Configuration { get; }

        private AppSettings AppSettings { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IValidateOptions<AppSettings>, AppSettingsValidation>());

            services.Configure<AppSettings>(Configuration);

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
                options.Secure = CookieSecurePolicy.None;
            });

            services.AddControllersWithViews(setupAction =>
            {
                setupAction.Filters.Add(typeof(CustomActionFilter));
                setupAction.Filters.Add(typeof(CustomResultFilter));
                setupAction.Filters.Add(typeof(CustomAuthorizationFilter));
                setupAction.Filters.Add(typeof(CustomExceptionFilter));
            })
            .AddNewtonsoftJson();

            services.AddPersistence(AppSettings.ConnectionStrings.ClassifiedAds)
                    .AddDomainServices()
                    .AddMessageHandlers();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.AccessDeniedPath = "/Authorization/AccessDenied";
            })
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.Authority = AppSettings.OpenIdConnect.Authority;
                options.ClientId = AppSettings.OpenIdConnect.ClientId;
                options.ClientSecret = AppSettings.OpenIdConnect.ClientSecret;
                options.ResponseType = "code id_token";
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("ClassifiedAds.WebAPI");
                options.Scope.Add("offline_access");
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.RequireHttpsMetadata = AppSettings.OpenIdConnect.RequireHttpsMetadata;
            });
            services.AddSingleton<IClaimsTransformation, CustomClaimsTransformation>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("CustomPolicy", policy =>
                {
                    policy.AddRequirements(new CustomRequirement());
                });
            })
            .AddSingleton<IAuthorizationHandler, CustomRequirementHandler>();

            services.AddTransient<ProfilingHttpHandler>();
            services.AddHttpClient(string.Empty)
                    .AddHttpMessageHandler<ProfilingHttpHandler>();

            services.AddMiniProfiler(options =>
            {
                options.RouteBasePath = "/profiler"; // access /profiler/results to see last profile check
                options.PopupRenderPosition = StackExchange.Profiling.RenderPosition.BottomLeft;
                options.PopupShowTimeWithChildren = true;
            })
            .AddEntityFramework();

            var healthChecksBuilder = services.AddHealthChecks()
                .AddSqlServer(connectionString: AppSettings.ConnectionStrings.ClassifiedAds,
                    healthQuery: "SELECT 1;",
                    name: "Sql Server",
                    failureStatus: HealthStatus.Degraded)
                .AddUrlGroup(new Uri(AppSettings.OpenIdConnect.Authority),
                    name: "Identity Server",
                    failureStatus: HealthStatus.Degraded)
                .AddUrlGroup(new Uri(AppSettings.ResourceServer.Endpoint),
                    name: "Resource (Web API) Server",
                    failureStatus: HealthStatus.Degraded)
                .AddSignalRHub(AppSettings.NotificationServer.Endpoint + "/HealthCheckHub",
                    name: "Notification (SignalR) Server",
                    failureStatus: HealthStatus.Degraded)
                .AddUrlGroup(new Uri(AppSettings.BackgroundServer.Endpoint),
                    name: "Background Services Server",
                    failureStatus: HealthStatus.Degraded);

            services.AddHealthChecksUI(setupSettings: setup =>
            {
                setup.AddHealthCheckEndpoint("Basic Health Check", $"{AppSettings.CurrentUrl}/healthcheck");
            });

            services.AddHangfire(x =>
            {
                x.UseSqlServerStorage(AppSettings.ConnectionStrings.ClassifiedAds);
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<ICurrentUser, CurrentWebUser>();
            services.AddTransient<IWebNotification, SignalRNotification>();

            if (AppSettings.Storage.UsedAzure())
            {
                services.AddSingleton<IFileStorageManager>(new AzureBlobStorageManager(AppSettings.Storage.Azure.ConnectionString, AppSettings.Storage.Azure.Container));

                healthChecksBuilder.AddAzureBlobStorage(
                    AppSettings.Storage.Azure.ConnectionString,
                    containerName: AppSettings.Storage.Azure.Container,
                    name: "Storage (Azure Blob)",
                    failureStatus: HealthStatus.Degraded);
            }
            else if (AppSettings.Storage.UsedAmazon())
            {
                services.AddSingleton<IFileStorageManager>(
                    new AmazonS3StorageManager(
                        AppSettings.Storage.Amazon.AccessKeyID,
                        AppSettings.Storage.Amazon.SecretAccessKey,
                        AppSettings.Storage.Amazon.BucketName,
                        AppSettings.Storage.Amazon.RegionEndpoint));

                healthChecksBuilder.AddS3(
                    s3 =>
                    {
                        s3.AccessKey = AppSettings.Storage.Amazon.AccessKeyID;
                        s3.SecretKey = AppSettings.Storage.Amazon.SecretAccessKey;
                        s3.BucketName = AppSettings.Storage.Amazon.BucketName;
                        s3.S3Config = new AmazonS3Config
                        {
                            RegionEndpoint = RegionEndpoint.GetBySystemName(AppSettings.Storage.Amazon.RegionEndpoint),
                        };
                    },
                    name: "Storage (Amazon S3)",
                    failureStatus: HealthStatus.Degraded);
            }
            else
            {
                services.AddSingleton<IFileStorageManager>(new LocalFileStorageManager(AppSettings.Storage.Local.Path));

                healthChecksBuilder.AddFilePathWrite(
                AppSettings.Storage.Local.Path,
                name: "Storage (Local Directory)",
                failureStatus: HealthStatus.Degraded);
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

                healthChecksBuilder.AddRabbitMQ(
                    rabbitMQConnectionString: AppSettings.MessageBroker.RabbitMQ.ConnectionString,
                    name: "Message Broker (RabbitMQ)",
                    failureStatus: HealthStatus.Degraded);
            }
            else if (AppSettings.MessageBroker.UsedKafka())
            {
                services.AddSingleton<IMessageSender<FileUploadedEvent>>(new KafkaSender<FileUploadedEvent>(
                    AppSettings.MessageBroker.Kafka.BootstrapServers,
                    AppSettings.MessageBroker.Kafka.Topic_FileUploaded));

                services.AddSingleton<IMessageSender<FileDeletedEvent>>(new KafkaSender<FileDeletedEvent>(
                    AppSettings.MessageBroker.Kafka.BootstrapServers,
                    AppSettings.MessageBroker.Kafka.Topic_FileDeleted));

                healthChecksBuilder.AddKafka(
                    setup =>
                    {
                        setup.BootstrapServers = AppSettings.MessageBroker.Kafka.BootstrapServers;
                        setup.MessageTimeoutMs = 5000;
                    },
                    name: "Message Broker (Kafka)",
                    failureStatus: HealthStatus.Degraded);
            }
            else if (AppSettings.MessageBroker.UsedAzureQueue())
            {
                services.AddSingleton<IMessageSender<FileUploadedEvent>>(new AzureQueueSender<FileUploadedEvent>(
                    connectionString: AppSettings.MessageBroker.AzureQueue.ConnectionString,
                    queueName: AppSettings.MessageBroker.AzureQueue.QueueName_FileUploaded));

                services.AddSingleton<IMessageSender<FileDeletedEvent>>(new AzureQueueSender<FileDeletedEvent>(
                    connectionString: AppSettings.MessageBroker.AzureQueue.ConnectionString,
                    queueName: AppSettings.MessageBroker.AzureQueue.QueueName_FileDeleted));

                healthChecksBuilder.AddAzureQueueStorage(
                    connectionString: AppSettings.MessageBroker.AzureQueue.ConnectionString,
                    queueName: AppSettings.MessageBroker.AzureQueue.QueueName_FileUploaded,
                    name: "Message Broker (Azure Queue) File Uploaded",
                    failureStatus: HealthStatus.Degraded);

                healthChecksBuilder.AddAzureQueueStorage(
                    connectionString: AppSettings.MessageBroker.AzureQueue.ConnectionString,
                    queueName: AppSettings.MessageBroker.AzureQueue.QueueName_FileDeleted,
                    name: "Message Broker (Azure Queue) File Deleted",
                    failureStatus: HealthStatus.Degraded);
            }
            else if (AppSettings.MessageBroker.UsedAzureServiceBus())
            {
                services.AddSingleton<IMessageSender<FileUploadedEvent>>(new AzureServiceBusSender<FileUploadedEvent>(
                    connectionString: AppSettings.MessageBroker.AzureServiceBus.ConnectionString,
                    queueName: AppSettings.MessageBroker.AzureServiceBus.QueueName_FileUploaded));

                services.AddSingleton<IMessageSender<FileDeletedEvent>>(new AzureServiceBusSender<FileDeletedEvent>(
                    connectionString: AppSettings.MessageBroker.AzureServiceBus.ConnectionString,
                    queueName: AppSettings.MessageBroker.AzureServiceBus.QueueName_FileDeleted));

                healthChecksBuilder.AddAzureServiceBusQueue(
                    connectionString: AppSettings.MessageBroker.AzureServiceBus.ConnectionString,
                    queueName: AppSettings.MessageBroker.AzureServiceBus.QueueName_FileUploaded,
                    name: "Message Broker (Azure Service Bus) File Uploaded",
                    failureStatus: HealthStatus.Degraded);

                healthChecksBuilder.AddAzureServiceBusQueue(
                    connectionString: AppSettings.MessageBroker.AzureServiceBus.ConnectionString,
                    queueName: AppSettings.MessageBroker.AzureServiceBus.QueueName_FileDeleted,
                    name: "Message Broker (Azure Service Bus) File Deleted",
                    failureStatus: HealthStatus.Degraded);
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.ApplicationServices.RegisterDomainEventHandlers();

            app.UseMiddleware<CustomMiddleware>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSecurityHeaders();

            app.UseIPFiltering();

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMiniProfiler();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHealthChecks("/healthcheck", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
                ResultStatusCodes =
                {
                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [HealthStatus.Degraded] = StatusCodes.Status500InternalServerError,
                    [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable,
                },
            });

            app.UseHealthChecksUI(); // /healthchecks-ui#/healthchecks

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireDashboardAuthorizationFilter() },
                IgnoreAntiforgeryToken = true,
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
