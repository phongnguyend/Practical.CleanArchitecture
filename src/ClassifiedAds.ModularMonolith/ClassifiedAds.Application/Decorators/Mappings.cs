using System;
using System.Collections.Generic;
using ClassifiedAds.Application.Decorators.AuditLog;
using ClassifiedAds.Application.Decorators.DatabaseRetry;

namespace ClassifiedAds.Application.Decorators
{
    internal static class Mappings
    {
        public static readonly IReadOnlyDictionary<Type, Type> AttributeToCommandHandler = new Dictionary<Type, Type>
        {
            [typeof(AuditLogAttribute)] = typeof(AuditLogCommandDecorator<>),
            [typeof(DatabaseRetryAttribute)] = typeof(DatabaseRetryCommandDecorator<>),
        };

        public static readonly IReadOnlyDictionary<Type, Type> AttributeToQueryHandler = new Dictionary<Type, Type>
        {
            [typeof(AuditLogAttribute)] = typeof(AuditLogQueryDecorator<,>),
            [typeof(DatabaseRetryAttribute)] = typeof(DatabaseRetryQueryDecorator<,>),
        };
    }
}