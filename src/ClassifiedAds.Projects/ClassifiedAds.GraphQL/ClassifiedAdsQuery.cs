using ClassifiedAds.DomainServices;
using ClassifiedAds.GraphQL.Types;
using GraphQL.Types;
using System;

namespace ClassifiedAds.GraphQL
{
    public class ClassifiedAdsQuery : ObjectGraphType<object>
    {
        public ClassifiedAdsQuery(IProductService productService)
        {
            Name = "Query";

            Field<ListGraphType<ProductType>>(
                "products",
                resolve: context => productService.GetProducts()
                );

            Field<ProductType>(
                "product",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "id" }),
                resolve: context =>
                {
                    var id = context.GetArgument<Guid>("id");
                    return productService.GetById(id);
                }
            );
        }
    }
}
