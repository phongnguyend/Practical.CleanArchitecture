using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Services;
using ClassifiedAds.GraphQL.Types;
using GraphQL.Types;
using System;

namespace ClassifiedAds.GraphQL
{
    public class ClassifiedAdsMutation : ObjectGraphType
    {
        public ClassifiedAdsMutation(IProductService productService)
        {
            Name = "Mutation";

            Field<ProductType>(
                "createProduct",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<ProductInputType>> { Name = "product" }
                ),
                resolve: context =>
                {
                    var product = context.GetArgument<Product>("product");
                    productService.Add(product);
                    return product;
                });

            Field<BooleanGraphType>(
                "deleteProduct",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "id" }),
                resolve: context =>
                {
                    var id = context.GetArgument<Guid>("id");
                    var product = productService.GetById(id);

                    if (product != null)
                    {
                        productService.Delete(product);
                    }

                    return true;
                });
        }
    }
}
