using System;
using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using ClassifiedAds.Infrastructure.MessageBrokers;
using ClassifiedAds.Infrastructure.Notification;
using ClassifiedAds.Services.Notification.DTOs;
using ClassifiedAds.Services.Notification.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ClassifiedAds.Services.Notification.Background
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
            services.AddDateTimeProvider();
            services.AddApplicationServices();

            var messageBrokerOptions = new MessageBrokerOptions();
            var notificationOptions = new NotificationOptions();

            Configuration.GetSection("MessageBroker").Bind(messageBrokerOptions);
            Configuration.GetSection("Notification").Bind(notificationOptions);

            services.AddNotificationModule(messageBrokerOptions, notificationOptions, Configuration.GetConnectionString("ClassifiedAds"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            RunMessageBrokerReceivers(app.ApplicationServices.CreateScope().ServiceProvider);

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }

        private void RunMessageBrokerReceivers(IServiceProvider serviceProvider)
        {
            var emailMessageCreatedMessageQueueReceiver = serviceProvider.GetService<IMessageReceiver<EmailMessageCreatedEvent>>();
            var smsMessageCreatedMessageQueueReceiver = serviceProvider.GetService<IMessageReceiver<SmsMessageCreatedEvent>>();

            var emailMessageService = serviceProvider.GetService<EmailMessageService>();

            emailMessageCreatedMessageQueueReceiver?.Receive(data =>
            {
                string message = data.Id.ToString();

                try
                {
                    emailMessageService.SendEmailMessage(data.Id);
                }
                catch (Exception ex)
                { }

            });

            var smsMessageService = serviceProvider.GetService<SmsMessageService>();

            smsMessageCreatedMessageQueueReceiver?.Receive(data =>
            {
                string message = data.Id.ToString();

                try
                {
                    smsMessageService.SendSmsMessage(data.Id);
                }
                catch (Exception ex)
                { }

            });
        }
    }
}
