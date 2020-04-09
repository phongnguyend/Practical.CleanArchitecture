using ClassifiedAds.Domain;
using ClassifiedAds.Domain.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DomainServicesCollectionExtensions
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(ICrudService<>), typeof(CrudService<>))
                    .AddScoped<IUserService, UserService>()
                    .AddScoped<IProductService, ProductService>();
            return services;
        }
    }
}
