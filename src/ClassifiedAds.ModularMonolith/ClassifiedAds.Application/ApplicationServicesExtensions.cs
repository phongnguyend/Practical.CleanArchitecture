using ClassifiedAds.Application;
using ClassifiedAds.Application.Decorators;
using ClassifiedAds.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
            List<Type> handlerTypes = assembly.GetTypes()
                .Where(x => x.GetInterfaces().Any(y => IsHandlerInterface(y)))
                .Where(x => x.Name.EndsWith("Handler"))
                .ToList();

            foreach (Type type in handlerTypes)
            {
                AddHandler(services, type);
            }

            services.AddEventHandlers(assembly);

            return services;
        }

        private static IServiceCollection AddEventHandlers(this IServiceCollection services, Assembly assembly)
        {
            var types = assembly.GetTypes()
                    .Where(x => x.GetInterfaces().Any(y => y.IsGenericType && y.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>)))
                    .ToList();

            foreach (Type type in types)
            {
                services.AddTransient(type);
            }

            return services;
        }

        private static void AddHandler(IServiceCollection services, Type type)
        {
            object[] attributes = type.GetCustomAttributes(false);

            List<Type> pipeline = attributes
                .Select(x => ToDecorator(type, x))
                .Concat(new[] { type })
                .Reverse()
                .ToList();

            var interfaceTypes = type.GetInterfaces().Where(y => IsHandlerInterface(y)).ToList();

            foreach (var interfaceType in interfaceTypes)
            {
                Func<IServiceProvider, object> factory = BuildPipeline(pipeline, interfaceType);

                services.AddTransient(interfaceType, factory);
            }
        }

        private static Func<IServiceProvider, object> BuildPipeline(List<Type> pipeline, Type interfaceType)
        {
            List<ConstructorInfo> ctors = pipeline
                .Select(x =>
                {
                    Type type = x.IsGenericType ? x.MakeGenericType(interfaceType.GenericTypeArguments) : x;
                    return type.GetConstructors().Single();
                })
                .ToList();

            Func<IServiceProvider, object> func = provider =>
            {
                object current = null;

                foreach (ConstructorInfo ctor in ctors)
                {
                    List<ParameterInfo> parameterInfos = ctor.GetParameters().ToList();

                    object[] parameters = GetParameters(parameterInfos, current, provider);

                    current = ctor.Invoke(parameters);
                }

                return current;
            };

            return func;
        }

        private static object[] GetParameters(List<ParameterInfo> parameterInfos, object current, IServiceProvider provider)
        {
            var result = new object[parameterInfos.Count];

            for (int i = 0; i < parameterInfos.Count; i++)
            {
                result[i] = GetParameter(parameterInfos[i], current, provider);
            }

            return result;
        }

        private static object GetParameter(ParameterInfo parameterInfo, object current, IServiceProvider provider)
        {
            Type parameterType = parameterInfo.ParameterType;

            if (IsHandlerInterface(parameterType))
            {
                return current;
            }

            object service = provider.GetService(parameterType);
            if (service != null)
            {
                return service;
            }

            throw new ArgumentException($"Type {parameterType} not found");
        }

        private static Type ToDecorator(Type type, object attribute)
        {
            Type attributeType = attribute.GetType();

            if (attributeType == typeof(DatabaseRetryAttribute))
            {
                if (HasInterface(type, typeof(ICommandHandler<>)))
                {
                    return typeof(DatabaseRetryCommandDecorator<>);
                }

                if (HasInterface(type, typeof(IQueryHandler<,>)))
                {
                    return typeof(DatabaseRetryQueryDecorator<,>);
                }
            }

            if (attributeType == typeof(AuditLogAttribute))
            {
                if (HasInterface(type, typeof(ICommandHandler<>)))
                {
                    return typeof(AuditLogCommandDecorator<>);
                }

                if (HasInterface(type, typeof(IQueryHandler<,>)))
                {
                    return typeof(AuditLogQueryDecorator<,>);
                }
            }

            throw new ArgumentException(attribute.ToString());
        }

        private static bool IsHandlerInterface(Type type)
        {
            if (!type.IsGenericType)
            {
                return false;
            }

            Type typeDefinition = type.GetGenericTypeDefinition();

            return typeDefinition == typeof(ICommandHandler<>) || typeDefinition == typeof(IQueryHandler<,>);
        }

        private static bool HasInterface(Type type, Type interfaceType)
        {
            return type.FindInterfaces((i, _) => i.GetGenericTypeDefinition() == interfaceType, null).Length > 0;
        }
    }
}
