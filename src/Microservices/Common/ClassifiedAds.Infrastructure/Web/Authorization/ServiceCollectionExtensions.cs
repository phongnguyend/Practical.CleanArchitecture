using ClassifiedAds.Infrastructure.Web.Authorization.Policies;
using ClassifiedAds.Infrastructure.Web.Authorization.Requirements;
using ClassifiedAds.Infrastructure.Web.ClaimsTransformations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services, Assembly assembly)
    {
        if (!services.Any(s => s.ServiceType == typeof(IClaimsTransformation) && s.ImplementationType == typeof(CustomClaimsTransformation)))
        {
            services.AddSingleton<IClaimsTransformation, CustomClaimsTransformation>();
        }

        if (!services.Any(s => s.ServiceType == typeof(IAuthorizationPolicyProvider) && s.ImplementationType == typeof(CustomAuthorizationPolicyProvider)))
        {
            services.AddSingleton<IAuthorizationPolicyProvider, CustomAuthorizationPolicyProvider>();
        }

        services.Configure<AuthorizationOptions>(options =>
        {
        });

        if (!services.Any(s => s.ServiceType == typeof(IAuthorizationHandler) && s.ImplementationType == typeof(PermissionRequirementHandler)))
        {
            services.AddSingleton<IAuthorizationHandler, PermissionRequirementHandler>();
        }

        var requirementHandlerTypes = assembly.GetTypes()
            .Where(IsAuthorizationHandler)
            .ToList();

        foreach (var type in requirementHandlerTypes)
        {
            services.AddSingleton(typeof(IAuthorizationHandler), type);
        }

        return services;
    }

    private static bool IsAuthorizationHandler(Type type)
    {
        if (type.BaseType == null)
        {
            return false;
        }

        if (!type.BaseType.IsGenericType)
        {
            return false;
        }

        var baseType = type.BaseType.GetGenericTypeDefinition();
        return baseType == typeof(AuthorizationHandler<>) || baseType == typeof(AuthorizationHandler<,>);
    }
}
