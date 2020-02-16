using ClassifiedAds.DomainServices;
using ClassifiedAds.DomainServices.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DomainServicesCollectionExtensions
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericService<>), typeof(GenericService<>))
                    .AddScoped<IUserService, UserService>()
                    .AddScoped<IProductService, ProductService>()
                    .AddScoped<IStoreService, StoreService>();
            return services;
        }
    }
}
