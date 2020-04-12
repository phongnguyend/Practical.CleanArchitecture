using ClassifiedAds.Domain.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DomainServicesCollectionExtensions
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.AddScoped<ProductService, ProductService>();
            return services;
        }
    }
}
