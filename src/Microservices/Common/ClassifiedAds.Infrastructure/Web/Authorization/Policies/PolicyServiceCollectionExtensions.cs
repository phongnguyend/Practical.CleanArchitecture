using ClassifiedAds.Infrastructure.Web.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class PolicyServiceCollectionExtensions
{
    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services, Assembly assembly, IEnumerable<string> policies)
    {
        services.Configure<AuthorizationOptions>(options =>
        {
            foreach (var policyName in policies)
            {
                options.AddPolicy(policyName, policy =>
                {
                    policy.AddRequirements(new PermissionRequirement
                    {
                        PermissionName = policyName
                    });
                });
            }
        });

        if (!services.Any(s => s.ServiceType == typeof(IAuthorizationHandler) && s.ImplementationType == typeof(PermissionRequirementHandler)))
        {
            services.AddSingleton(typeof(IAuthorizationHandler), typeof(PermissionRequirementHandler));
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
