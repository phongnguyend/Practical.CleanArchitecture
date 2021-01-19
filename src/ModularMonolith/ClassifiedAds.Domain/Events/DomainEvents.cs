﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ClassifiedAds.Domain.Events
{
    public class DomainEvents : IDomainEvents
    {
        private static List<Type> _handlers = new List<Type>();
        private static IServiceProvider _serviceProvider;

        public static void RegisterHandlers(Assembly assembly, IServiceCollection services)
        {
            var types = assembly.GetTypes()
                                .Where(x => x.GetInterfaces().Any(y => y.IsGenericType && y.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>)))
                                .ToList();

            foreach (var type in types)
            {
                services.AddTransient(type);
            }

            _handlers.AddRange(types);
        }

        public DomainEvents(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task DispatchAsync(IDomainEvent domainEvent)
        {
            foreach (Type handlerType in _handlers)
            {
                bool canHandleEvent = handlerType.GetInterfaces()
                    .Any(x => x.IsGenericType
                        && x.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>)
                        && x.GenericTypeArguments[0] == domainEvent.GetType());

                if (canHandleEvent)
                {
                    dynamic handler = _serviceProvider.GetService(handlerType);
                    await handler.HandleAsync((dynamic)domainEvent);
                }
            }
        }
    }
}
