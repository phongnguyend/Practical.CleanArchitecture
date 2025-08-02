using ClassifiedAds.Application;
using ClassifiedAds.Application.Products.Services;
using ClassifiedAds.Application.Users.Services;
using ClassifiedAds.Domain.Entities;
using System;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class ApplicationServicesExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, Action<Type, Type, ServiceLifetime> configureInterceptor = null)
    {
        services.AddScoped(typeof(ICrudService<>), typeof(CrudService<>))
            .AddScoped<IUserService, UserService>()
            .AddScoped<IProductService, ProductService>();

        if (configureInterceptor != null)
        {
            var aggregateRootTypes = typeof(IAggregateRoot).Assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(Entity<Guid>)) && x.GetInterfaces().Contains(typeof(IAggregateRoot))).ToList();
            foreach (var type in aggregateRootTypes)
            {
                configureInterceptor(typeof(ICrudService<>).MakeGenericType(type), typeof(CrudService<>).MakeGenericType(type), ServiceLifetime.Scoped);
            }

            configureInterceptor(typeof(IUserService), typeof(UserService), ServiceLifetime.Scoped);
            configureInterceptor(typeof(IProductService), typeof(ProductService), ServiceLifetime.Scoped);
        }

        return services;
    }

    public static IServiceCollection AddMessageHandlers(this IServiceCollection services)
    {
        services.AddScoped<Dispatcher>();

        var assembly = Assembly.GetExecutingAssembly();

        var assemblyTypes = assembly.GetTypes();

        foreach (var type in assemblyTypes)
        {
            var handlerInterfaces = type.GetInterfaces()
               .Where(Utils.IsHandlerInterface)
               .ToList();

            if (!handlerInterfaces.Any())
            {
                continue;
            }

            var handlerFactory = new HandlerFactory(type);
            foreach (var interfaceType in handlerInterfaces)
            {
                services.AddTransient(interfaceType, provider => handlerFactory.Create(provider, interfaceType));
            }
        }

        var aggregateRootTypes = typeof(IAggregateRoot).Assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(Entity<Guid>)) && x.GetInterfaces().Contains(typeof(IAggregateRoot))).ToList();

        var genericHandlerTypes = new[]
        {
            typeof(GetEntititesQueryHandler<>),
            typeof(GetEntityByIdQueryHandler<>),
            typeof(AddOrUpdateEntityCommandHandler<>),
            typeof(DeleteEntityCommandHandler<>),
        };

        foreach (var aggregateRootType in aggregateRootTypes)
        {
            foreach (var genericHandlerType in genericHandlerTypes)
            {
                var handlerType = genericHandlerType.MakeGenericType(aggregateRootType);
                var handlerInterfaces = handlerType.GetInterfaces();

                var handlerFactory = new HandlerFactory(handlerType);
                foreach (var interfaceType in handlerInterfaces)
                {
                    services.AddTransient(interfaceType, provider => handlerFactory.Create(provider, interfaceType));
                }
            }
        }

        Dispatcher.RegisterEventHandlers(assembly, services);

        return services;
    }

    public static Dispatcher GetDispatcher(this IServiceProvider provider)
    {
        return provider.GetRequiredService<Dispatcher>();
    }
}
