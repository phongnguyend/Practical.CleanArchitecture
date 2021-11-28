using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using ClassifiedAds.Infrastructure.DistributedTracing;
using ClassifiedAds.Infrastructure.Web.Filters;
using ClassifiedAds.Services.Storage.ConfigurationOptions;
using ClassifiedAds.Services.Storage.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Storage
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
            AppSettings.ConnectionStrings.MigrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddControllers(configure =>
            {
                configure.Filters.Add(typeof(GlobalExceptionFilter));
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAnyOrigin", builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

            services.AddDistributedTracing(AppSettings.DistributedTracing);

            services.AddDateTimeProvider();
            services.AddApplicationServices();

            services.AddStorageModule(AppSettings);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.Authority = AppSettings.IdentityServerAuthentication.Authority;
                        options.Audience = AppSettings.IdentityServerAuthentication.ApiName;
                        options.RequireHttpsMetadata = AppSettings.IdentityServerAuthentication.RequireHttpsMetadata;
                    });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Policy.Handle<Exception>().WaitAndRetry(new[]
            {
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(20),
                TimeSpan.FromSeconds(30),
            })
            .Execute(() =>
            {
                app.MigrateStorageDb();
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            RunMessageBrokerReceivers(app.ApplicationServices.CreateScope().ServiceProvider);

            app.UseRouting();

            app.UseCors("AllowAnyOrigin");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void RunMessageBrokerReceivers(IServiceProvider serviceProvider)
        {
            var fileUploadedMessageQueueReceiver = serviceProvider.GetService<IMessageReceiver<FileUploadedEvent>>();
            var fileDeletedMessageQueueReceiver = serviceProvider.GetService<IMessageReceiver<FileDeletedEvent>>();

            fileUploadedMessageQueueReceiver?.Receive(async (data, metaData) =>
            {
                await Task.Delay(5000); // simulate long running task

                string message = data.FileEntry.Id.ToString();
            });

            fileDeletedMessageQueueReceiver?.Receive(async (data, metaData) =>
            {
                await Task.Delay(5000); // simulate long running task

                string message = data.FileEntry.Id.ToString();
            });
        }
    }
}
