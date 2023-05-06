using System;
using System.Linq;
using System.Text;

namespace ClassifiedAds.CrossCuttingConcerns.ExtensionMethods;

public static class TypeExtensions
{
    public static bool HasInterface(this Type type, Type interfaceType)
    {
        return type.GetInterfacesOf(interfaceType).Length > 0;
    }

    public static Type[] GetInterfacesOf(this Type type, Type interfaceType)
    {
        return type.FindInterfaces((i, _) => i.GetGenericTypeDefinitionSafe() == interfaceType, null);
    }

    public static Type GetGenericTypeDefinitionSafe(this Type type)
    {
        return type.IsGenericType
            ? type.GetGenericTypeDefinition()
            : type;
    }

    public static Type MakeGenericTypeSafe(this Type type, params Type[] typeArguments)
    {
        return type.IsGenericType && !type.GenericTypeArguments.Any()
            ? type.MakeGenericType(typeArguments)
            : type;
    }

    public static string GenerateMappingCode(this Type type)
    {
        var names = type.GetProperties().Select(x => x.Name);

        var text1 = new StringBuilder();
        var text2 = new StringBuilder();
        var text3 = new StringBuilder();
        var text4 = new StringBuilder();

        foreach (var name in names)
        {
            text1.Append($"a.{name} = {name};{Environment.NewLine}");
            text2.Append($"{name} = b.{name};{Environment.NewLine}");
            text3.Append($"{name} = b.{name},{Environment.NewLine}");
            text4.Append($"a.{name} = b.{name};{Environment.NewLine}");
        }

        return text1.ToString()
            + "--------------------------------------" + Environment.NewLine
            + text2.ToString()
            + "--------------------------------------" + Environment.NewLine
            + text3.ToString()
            + "--------------------------------------" + Environment.NewLine
            + text4.ToString();
    }
}
