using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ClassifiedAds.Application.Decorators;
using ClassifiedAds.Application.Decorators.Core;
using ClassifiedAds.CrossCuttingConcerns.ExtensionMethods;

namespace ClassifiedAds.Application.Core
{
    internal class HandlerFactory
    {
        private readonly List<Func<object, Type, IServiceProvider, object>> _handlerFactoriesPipeline;

        public HandlerFactory(Type type)
        {
            _handlerFactoriesPipeline = new List<Func<object, Type, IServiceProvider, object>>();
            AddHandlerFactory(type);
            AddDecoratedFactories(type);
        }

        public object Create(IServiceProvider provider, Type handlerInterfaceType)
        {
            object currentHandler = null;
            foreach (var handlerFactory in _handlerFactoriesPipeline)
            {
                currentHandler = handlerFactory(currentHandler, handlerInterfaceType, provider);
            }

            return currentHandler;
        }

        private void AddDecoratedFactories(Type type)
        {
            var attributes = type.GetCustomAttributes(inherit: false);

            for (var i = attributes.Length - 1; i >= 0; i--)
            {
                var attribute = attributes[i];
                var attributeType = attribute.GetType();
                Type decoratorHandlerType;
                if (type.HasInterface(typeof(ICommandHandler<>)))
                {
                    if (!Mappings.AttributeToCommandHandler.TryGetValue(
                        attributeType,
                        out decoratorHandlerType))
                    {
                        throw new NotSupportedException($"Please add mapping for {attributeType} to {nameof(Mappings)}.{nameof(Mappings.AttributeToCommandHandler)}.");
                    }
                }
                else if (type.HasInterface(typeof(IQueryHandler<,>)))
                {
                    if (!Mappings.AttributeToQueryHandler.TryGetValue(
                        attributeType,
                        out decoratorHandlerType))
                    {
                        throw new NotSupportedException($"Please add mapping for {attributeType} to {nameof(Mappings)}.{nameof(Mappings.AttributeToQueryHandler)}.");
                    }
                }
                else
                {
                    throw new NotSupportedException();
                }

                AddHandlerFactory(decoratorHandlerType, attribute);
            }
        }

        private void AddHandlerFactory(Type handlerType, object attribute = null)
        {
            _handlerFactoriesPipeline.Add(CreateHandler);

            object CreateHandler(object decoratingHandler, Type interfaceType, IServiceProvider provider)
            {
                var ctor = handlerType
                   .MakeGenericTypeSafe(interfaceType.GenericTypeArguments)
                   .GetConstructors()
                   .Single();
                var parameterInfos = ctor.GetParameters();
                var parameters = GetParameters(parameterInfos, decoratingHandler, provider);

                var handler = ctor.Invoke(parameters);

                if (attribute != null
                    && attribute is ISettingsProvider settingsProvider
                    && handler is ISettingsAcceptable settingsAcceptable)
                {
                    settingsAcceptable.Accept(settingsProvider);
                }

                return handler;
            }
        }

        private static object[] GetParameters(IEnumerable<ParameterInfo> parameterInfos, object current, IServiceProvider provider)
        {
            return parameterInfos.Select(GetParameter).ToArray();

            object GetParameter(ParameterInfo parameterInfo)
            {
                var parameterType = parameterInfo.ParameterType;

                if (Utils.IsHandlerInterface(parameterType))
                {
                    return current;
                }

                var service = provider.GetService(parameterType);
                if (service != null)
                {
                    return service;
                }

                throw new ArgumentException($"Type {parameterType} not found");
            }
        }
    }
}