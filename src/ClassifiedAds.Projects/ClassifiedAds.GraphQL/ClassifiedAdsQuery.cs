using ClassifiedAds.Domain.Services;
using ClassifiedAds.GraphQL.Types;
using ClassifiedAds.GRPC;
using GraphQL.Types;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;

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
                    var client = GetGrpcClient(configuration);
                    var productsResponse = client.GetProducts(new GetProductsRequest());

                    var products = new List<Domain.Entities.Product>();

                    foreach (var productMessage in productsResponse.Products)
                    {
                        products.Add(new Domain.Entities.Product
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

        private static Product.ProductClient GetGrpcClient(IConfiguration configuration)
        {
            var channel = GrpcChannel.ForAddress(configuration["GrpcService:Endpoint"],
                new GrpcChannelOptions
                {
                    HttpClient = new HttpClient(new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                        {
                            // TODO: verify the Certificate
                            return true;
                        }
                    })
                });

            var client = new Product.ProductClient(channel);
            return client;
        }
    }
}
