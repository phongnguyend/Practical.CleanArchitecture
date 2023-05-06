using Castle.DynamicProxy;
using ClassifiedAds.Infrastructure.Interceptors;
using System;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionServiceExtensions
{
    public static IServiceCollection ConfigureInterceptors(this IServiceCollection services)
    {
        services.AddSingleton(new ProxyGenerator());
        services.AddTransient<LoggingInterceptor>();
        services.AddTransient<ErrorCatchingInterceptor>();
        return services;
    }

    public static IServiceCollection AddInterceptors(this IServiceCollection services, Type serviceType, Type implementationType, ServiceLifetime serviceLifetime, Type[] interceptorTypes)
    {
        if (interceptorTypes == null || !interceptorTypes.Any())
        {
            return services;
        }

        services.Add(new ServiceDescriptor(implementationType, implementationType, serviceLifetime));

        var serviceDescriptor = new ServiceDescriptor(serviceType, provider =>
        {
            var target = provider.GetService(implementationType);

            var interceptors = interceptorTypes.Select(x => (IInterceptor)provider.GetService(x)).ToArray();

            var proxy = provider.GetService<ProxyGenerator>().CreateInterfaceProxyWithTarget(serviceType, target, interceptors);
            return proxy;
        }, serviceLifetime);

        services.Add(serviceDescriptor);

        return services;
    }

    public static IServiceCollection AddInterceptors(this IServiceCollection services, Type serviceType, Type implementationType, ServiceLifetime serviceLifetime, InterceptorsOptions interceptorsOptions)
    {
        return services.AddInterceptors(serviceType, implementationType, serviceLifetime, interceptorsOptions?.GetInterceptors());
    }
}
