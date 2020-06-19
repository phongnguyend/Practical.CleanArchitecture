using System.Linq;
using System.Reflection;
using ClassifiedAds.Application;
using ClassifiedAds.Application.Core;
using ClassifiedAds.Domain.Events;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<Dispatcher>();

            services.AddSingleton<IDomainEvents, DomainEvents>();

            return services;
        }

        public static IServiceCollection AddMessageHandlers(this IServiceCollection services, Assembly assembly)
        {
            var assemblyTypes = assembly.GetTypes();

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
