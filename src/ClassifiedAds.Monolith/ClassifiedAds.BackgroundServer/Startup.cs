using System;
using System.IO;
using System.Threading;
using ClassifiedAds.Application.BackgroundTasks;
using ClassifiedAds.Application.FileEntries.Events;
using ClassifiedAds.BackgroundServer.ConfigurationOptions;
using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using ClassifiedAds.Domain.Notification;
using ClassifiedAds.Infrastructure.HealthChecks;
using ClassifiedAds.Infrastructure.Logging;
using ClassifiedAds.Infrastructure.MessageBrokers.AzureQueue;
using ClassifiedAds.Infrastructure.MessageBrokers.AzureServiceBus;
using ClassifiedAds.Infrastructure.MessageBrokers.Kafka;
using ClassifiedAds.Infrastructure.MessageBrokers.RabbitMQ;
using ClassifiedAds.Infrastructure.Notification;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ClassifiedAds.BackgroundServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;

            env.UseClassifiedAdsLogger();
        }

        public IConfiguration Configuration { get; }

        private AppSettings AppSettings { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var appSettings = new AppSettings();
            Configuration.Bind(appSettings);
            AppSettings = appSettings;

            var validationResult = appSettings.Validate();
            if (validationResult.Failed)
            {
                throw new Exception(validationResult.FailureMessage);
            }

            services.Configure<AppSettings>(Configuration);

            if (appSettings.CheckDependency.Enabled)
            {
                var hosts = appSettings.CheckDependency.Host.Split(',');
                foreach (var host in hosts)
                {
                    NetworkPortCheck.Wait(host, 5);
                }
            }

            services.AddHangfire(x =>
            {
                var options = new SqlServerStorageOptions
                {
                    PrepareSchemaIfNecessary = true,
                };
                x.UseSqlServerStorage(appSettings.ConnectionStrings.ClassifiedAds, options);
            });

            services.AddTransient<IWebNotification, SignalRNotification>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHangfireServer();

            RecurringJob.AddOrUpdate<SendEmail>(job => job.Run(), Cron.Minutely);
            RecurringJob.AddOrUpdate<SendSms>(job => job.Run(), Cron.Minutely);
            RecurringJob.AddOrUpdate<SimulatedLongRunningJob>(job => job.Run(AppSettings.NotificationServer.Endpoint), Cron.Minutely);

            RunMessageBrokerReceivers();

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello Hangfire Server!");
            });
        }

        private void RunMessageBrokerReceivers()
        {
            IMessageReceiver<FileUploadedEvent> fileUploadedMessageQueueReceiver = null;
            IMessageReceiver<FileDeletedEvent> fileDeletedMessageQueueReceiver = null;

            if (AppSettings.MessageBroker.UsedRabbitMQ())
            {
                fileUploadedMessageQueueReceiver = new RabbitMQReceiver<FileUploadedEvent>(new RabbitMQReceiverOptions
                {
                    HostName = AppSettings.MessageBroker.RabbitMQ.HostName,
                    UserName = AppSettings.MessageBroker.RabbitMQ.UserName,
                    Password = AppSettings.MessageBroker.RabbitMQ.Password,
                    QueueName = AppSettings.MessageBroker.RabbitMQ.QueueName_FileUploaded,
                    AutomaticCreateEnabled = true,
                    ExchangeName = AppSettings.MessageBroker.RabbitMQ.ExchangeName,
                    RoutingKey = AppSettings.MessageBroker.RabbitMQ.RoutingKey_FileUploaded,
                });

                fileDeletedMessageQueueReceiver = new RabbitMQReceiver<FileDeletedEvent>(new RabbitMQReceiverOptions
                {
                    HostName = AppSettings.MessageBroker.RabbitMQ.HostName,
                    UserName = AppSettings.MessageBroker.RabbitMQ.UserName,
                    Password = AppSettings.MessageBroker.RabbitMQ.Password,
                    QueueName = AppSettings.MessageBroker.RabbitMQ.QueueName_FileDeleted,
                    AutomaticCreateEnabled = true,
                    ExchangeName = AppSettings.MessageBroker.RabbitMQ.ExchangeName,
                    RoutingKey = AppSettings.MessageBroker.RabbitMQ.RoutingKey_FileDeleted,
                });
            }

            if (AppSettings.MessageBroker.UsedKafka())
            {
                fileUploadedMessageQueueReceiver = new KafkaReceiver<FileUploadedEvent>(
                    AppSettings.MessageBroker.Kafka.BootstrapServers,
                    AppSettings.MessageBroker.Kafka.Topic_FileUploaded,
                    AppSettings.MessageBroker.Kafka.GroupId);

                fileDeletedMessageQueueReceiver = new KafkaReceiver<FileDeletedEvent>(
                    AppSettings.MessageBroker.Kafka.BootstrapServers,
                    AppSettings.MessageBroker.Kafka.Topic_FileDeleted,
                    AppSettings.MessageBroker.Kafka.GroupId);
            }

            if (AppSettings.MessageBroker.UsedAzureQueue())
            {
                fileUploadedMessageQueueReceiver = new AzureQueueReceiver<FileUploadedEvent>(
                    AppSettings.MessageBroker.AzureQueue.ConnectionString,
                    AppSettings.MessageBroker.AzureQueue.QueueName_FileUploaded);

                fileDeletedMessageQueueReceiver = new AzureQueueReceiver<FileDeletedEvent>(
                    AppSettings.MessageBroker.AzureQueue.ConnectionString,
                    AppSettings.MessageBroker.AzureQueue.QueueName_FileDeleted);
            }

            if (AppSettings.MessageBroker.UsedAzureServiceBus())
            {
                fileUploadedMessageQueueReceiver = new AzureServiceBusReceiver<FileUploadedEvent>(
                    AppSettings.MessageBroker.AzureServiceBus.ConnectionString,
                    AppSettings.MessageBroker.AzureServiceBus.QueueName_FileUploaded);

                fileDeletedMessageQueueReceiver = new AzureServiceBusReceiver<FileDeletedEvent>(
                    AppSettings.MessageBroker.AzureServiceBus.ConnectionString,
                    AppSettings.MessageBroker.AzureServiceBus.QueueName_FileDeleted);
            }

            var notification = new SignalRNotification();
            var endpoint = $"{AppSettings.NotificationServer.Endpoint}/SimulatedLongRunningTaskHub";

            fileUploadedMessageQueueReceiver?.Receive(data =>
            {
                Thread.Sleep(5000); // simulate long running task

                string message = data.FileEntry.Id.ToString();

                notification.Send(endpoint, "SendTaskStatus", new { Step = $"{AppSettings.MessageBroker.Provider} - File Uploaded", Message = message });

            });

            fileDeletedMessageQueueReceiver?.Receive(data =>
            {
                Thread.Sleep(5000); // simulate long running task

                string message = data.FileEntry.Id.ToString();

                notification.Send(endpoint, "SendTaskStatus", new { Step = $"{AppSettings.MessageBroker.Provider} - File Deleted", Message = message });
            });
        }
    }
}
