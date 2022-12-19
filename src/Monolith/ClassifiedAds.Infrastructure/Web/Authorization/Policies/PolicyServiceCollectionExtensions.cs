using ClassifiedAds.Infrastructure.Web.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
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

            services.AddSingleton(typeof(IAuthorizationHandler), typeof(PermissionRequirementHandler));

            var requirementHandlerTypes = assembly.GetTypes()
                .Where(t => t.BaseType != null && t.BaseType.IsGenericType && t.BaseType.GetGenericTypeDefinition() == typeof(AuthorizationHandler<>))
                .ToList();

            foreach (var type in requirementHandlerTypes)
            {
                services.AddSingleton(typeof(IAuthorizationHandler), type);
            }

            return services;
        }
    }
}
