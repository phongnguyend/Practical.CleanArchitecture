using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using ClassifiedAds.Services.Notification.ConfigurationOptions;
using ClassifiedAds.Services.Notification.DTOs;
using ClassifiedAds.Services.Notification.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace ClassifiedAds.Services.Notification.Background
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
            services.AddDateTimeProvider();
            services.AddApplicationServices();

            services.AddNotificationModule(AppSettings);
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

            emailMessageCreatedMessageQueueReceiver?.Receive(async (data, metaData) =>
            {
                string message = data.Id.ToString();

                try
                {
                    await emailMessageService.SendEmailMessageAsync(data.Id);
                }
                catch (Exception ex)
                { }

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

            });
        }
    }
}
