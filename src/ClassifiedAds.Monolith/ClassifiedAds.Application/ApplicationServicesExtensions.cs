using ClassifiedAds.Application;
using ClassifiedAds.Application.Core;
using ClassifiedAds.Application.Products.Services;
using ClassifiedAds.Application.Users.Services;
using ClassifiedAds.Domain.Events;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            DomainEvents.RegisterHandlers(Assembly.GetExecutingAssembly(), services);

            services
                .AddSingleton<IDomainEvents, DomainEvents>()
                .AddScoped(typeof(ICrudService<>), typeof(CrudService<>))
                .AddScoped<IUserService, UserService>()
                .AddScoped<IProductService, ProductService>();

            return services;
        }

        public static IServiceCollection AddMessageHandlers(this IServiceCollection services)
        {
            services.AddScoped<Dispatcher>();

            var assemblyTypes = Assembly.GetExecutingAssembly().GetTypes();

            foreach (var type in assemblyTypes)
            {
                var handlerInterfaces = type.GetInterfaces()
                   .Where(Utils.IsHandlerInterface)
                   .ToList();

                if (handlerInterfaces.Any())
                {
                    var handlerFactory = new HandlerFactory(type);
                    foreach (var interfaceType in handlerInterfaces)
                    {
                        services.AddTransient(interfaceType, provider => handlerFactory.Create(provider, interfaceType));
                    }
                }
            }

            return services;
        }
    }
}
