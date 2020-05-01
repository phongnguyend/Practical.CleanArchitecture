using System;
using ClassifiedAds.Domain.Events;

namespace ClassifiedAds.Application.Core
{
    internal static class Utils
    {
        public static bool IsHandlerInterface(Type type)
        {
            if (!type.IsGenericType)
            {
                return false;
            }

            var typeDefinition = type.GetGenericTypeDefinition();

            return typeDefinition == typeof(ICommandHandler<>)
                || typeDefinition == typeof(IQueryHandler<,>);
        }

        public static bool IsEventHandlerInterface(Type type)
        {
            if (!type.IsGenericType)
            {
                return false;
            }

            var typeDefinition = type.GetGenericTypeDefinition();

            return typeDefinition == typeof(IDomainEventHandler<>);
        }
    }
}