using ClassifiedAds.DomainServices;
using ClassifiedAds.GraphQL.Types;
using ClassifiedAds.GRPC;
using GraphQL.Types;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace ClassifiedAds.GraphQL
{
    public class ClassifiedAdsQuery : ObjectGraphType<object>
    {
        public ClassifiedAdsQuery(IProductService productService, IConfiguration configuration)
        {
            Name = "Query";

            Field<ListGraphType<ProductType>>(
                "products",
                resolve: context =>
                {
                    var channel = GrpcChannel.ForAddress(configuration["GrpcEnpoint"]);
                    var client = new Product.ProductClient(channel);
                    var productsResponse = client.GetProducts(new GetProductsRequest());

                    var products = new List<DomainServices.Entities.Product>();

                    foreach (var productMessage in productsResponse.Products)
                    {
                        products.Add(new DomainServices.Entities.Product
                        {
                            Id = Guid.Parse(productMessage.Id),
                            Code = productMessage.Code,
                            Name = productMessage.Name,
                            Description = productMessage.Description
                        });
                    }

                    return products;
                }
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
