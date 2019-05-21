using ClassifiedAds.DomainServices;
using ClassifiedAds.DomainServices.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DomainServicesServiceCollectionExtensions
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>()
                    .AddScoped<IProductService, ProductService>()
                    .AddScoped<ISmsMessageService, SmsMessageService>()
                    .AddScoped<IEmailMessageService, EmailMessageService>();
            return services;
        }
    }
}
