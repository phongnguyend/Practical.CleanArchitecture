using ClassifiedAds.Application.BackgroundTasks;
using ClassifiedAds.Application.EmailMessages.DTOs;
using ClassifiedAds.Application.EmailMessages.Services;
using ClassifiedAds.Application.FileEntries.DTOs;
using ClassifiedAds.Application.SmsMessages.DTOs;
using ClassifiedAds.Application.SmsMessages.Services;
using ClassifiedAds.BackgroundServer.ConfigurationOptions;
using ClassifiedAds.BackgroundServer.HostedServices;
using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using ClassifiedAds.Domain.Notification;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ClassifiedAds.BackgroundServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;

            var appSettings = new AppSettings();
            Configuration.Bind(appSettings);
            AppSettings = appSettings;

            var validationResult = appSettings.Validate();
            if (validationResult.Failed)
            {
                throw new Exception(validationResult.FailureMessage);
            }
        }

        public IConfiguration Configuration { get; }

        private AppSettings AppSettings { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppSettings>(Configuration);

            services.AddDateTimeProvider();
            services.AddPersistence(AppSettings.ConnectionStrings.ClassifiedAds)
                    .AddDomainServices()
                    .AddApplicationServices();

            services.AddMessageBusSender<FileUploadedEvent>(AppSettings.MessageBroker);
            services.AddMessageBusSender<FileDeletedEvent>(AppSettings.MessageBroker);
            services.AddMessageBusSender<EmailMessageCreatedEvent>(AppSettings.MessageBroker);
            services.AddMessageBusSender<SmsMessageCreatedEvent>(AppSettings.MessageBroker);

            services.AddMessageBusReceiver<FileUploadedEvent>(AppSettings.MessageBroker);
            services.AddMessageBusReceiver<FileDeletedEvent>(AppSettings.MessageBroker);
            services.AddMessageBusReceiver<EmailMessageCreatedEvent>(AppSettings.MessageBroker);
            services.AddMessageBusReceiver<SmsMessageCreatedEvent>(AppSettings.MessageBroker);

            services.AddNotificationServices(AppSettings.Notification);

            services.AddWebNotification<SendTaskStatusMessage>(AppSettings.Notification.Web);

            services.AddHostedService<ResendEmailHostedService>();
            services.AddHostedService<ResendSmsHostedService>();
            services.AddHostedService<ScheduleCronJobHostedService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            RunMessageBrokerReceivers(app.ApplicationServices.CreateScope().ServiceProvider);

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello Background Server!");
            });
        }

        private void RunMessageBrokerReceivers(IServiceProvider serviceProvider)
        {
            var fileUploadedMessageQueueReceiver = serviceProvider.GetService<IMessageReceiver<FileUploadedEvent>>();
            var fileDeletedMessageQueueReceiver = serviceProvider.GetService<IMessageReceiver<FileDeletedEvent>>();
            var emailMessageCreatedMessageQueueReceiver = serviceProvider.GetService<IMessageReceiver<EmailMessageCreatedEvent>>();
            var smsMessageCreatedMessageQueueReceiver = serviceProvider.GetService<IMessageReceiver<SmsMessageCreatedEvent>>();

            var notification = serviceProvider.GetService<IWebNotification<SendTaskStatusMessage>>();

            fileUploadedMessageQueueReceiver?.Receive(async (data, metaData) =>
            {
                await Task.Delay(5000); // simulate long running task

                string message = data.FileEntry.Id.ToString();

                await notification.SendAsync(new SendTaskStatusMessage { Step = $"{AppSettings.MessageBroker.Provider} - File Uploaded", Message = message });
            });

            fileDeletedMessageQueueReceiver?.Receive(async (data, metaData) =>
            {
                await Task.Delay(5000); // simulate long running task

                string message = data.FileEntry.Id.ToString();

                await notification.SendAsync(new SendTaskStatusMessage { Step = $"{AppSettings.MessageBroker.Provider} - File Deleted", Message = message });
            });

            var emailMessageService = serviceProvider.GetService<EmailMessageService>();

            emailMessageCreatedMessageQueueReceiver?.Receive(async (data, metaData) =>
            {
                string message = data.Id.ToString();

                try
                {
                    await emailMessageService.SendEmailMessageAsync(data.Id);
                }
                catch (Exception ex)
                { }

                await notification.SendAsync(new SendTaskStatusMessage { Step = $"Send Email", Message = message });
            });

            var smsMessageService = serviceProvider.GetService<SmsMessageService>();

            smsMessageCreatedMessageQueueReceiver?.Receive(async (data, metaData) =>
            {
                string message = data.Id.ToString();

                try
                {
                    await smsMessageService.SendSmsMessageAsync(data.Id);
                }
                catch (Exception ex)
                { }

                await notification.SendAsync(new SendTaskStatusMessage { Step = $"Send Sms", Message = message });
            });
        }
    }
}
