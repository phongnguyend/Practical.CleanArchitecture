using ClassifiedAds.DomainServices;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DomainServicesServiceCollectionExtensions
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>()
                    .AddScoped<IProductService, ProductService>();
            return services;
        }
    }
}
