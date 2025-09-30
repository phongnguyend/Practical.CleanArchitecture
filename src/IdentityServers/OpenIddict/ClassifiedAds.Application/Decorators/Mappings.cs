using ClassifiedAds.CrossCuttingConcerns.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ClassifiedAds.Application.Decorators;

internal static class Mappings
{
    static Mappings()
    {
        var decorators = Assembly.GetExecutingAssembly().GetTypes();
        foreach (var type in decorators)
        {
            if (type.HasInterface(typeof(ICommandHandler<>)))
            {
                var decoratorAttribute = (MappingAttribute)type.GetCustomAttributes(false).FirstOrDefault(x => x.GetType() == typeof(MappingAttribute));

                if (decoratorAttribute != null)
                {
                    AttributeToCommandHandler[decoratorAttribute.Type] = type;
                }
            }
            else if (type.HasInterface(typeof(IQueryHandler<,>)))
            {
                var decoratorAttribute = (MappingAttribute)type.GetCustomAttributes(false).FirstOrDefault(x => x.GetType() == typeof(MappingAttribute));

                if (decoratorAttribute != null)
                {
                    AttributeToQueryHandler[decoratorAttribute.Type] = type;
                }
            }
        }
    }

    public static readonly Dictionary<Type, Type> AttributeToCommandHandler = new Dictionary<Type, Type>();

    public static readonly Dictionary<Type, Type> AttributeToQueryHandler = new Dictionary<Type, Type>();
}

[AttributeUsage(AttributeTargets.Class)]
public sealed class MappingAttribute : Attribute
{
    public Type Type { get; set; }
}