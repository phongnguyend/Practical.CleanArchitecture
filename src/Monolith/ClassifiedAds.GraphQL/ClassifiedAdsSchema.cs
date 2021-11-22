using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ClassifiedAds.GraphQL
{
    public class ClassifiedAdsSchema : Schema
    {
        public ClassifiedAdsSchema(IServiceProvider provider)
            : base(provider)
        {
            Query = provider.GetRequiredService<ClassifiedAdsQuery>();
            Mutation = provider.GetRequiredService<ClassifiedAdsMutation>();
        }
    }
}
