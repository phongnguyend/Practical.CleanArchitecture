using ClassifiedAds.Domain.Entities;
using GraphQL.Types;

namespace ClassifiedAds.GraphQL.Types
{
    public class ProductType : ObjectGraphType<Product>
    {
        public ProductType()
        {
            Field(h => h.Id).Description("The id of the product.");
            Field(h => h.Code).Description("The code of the product.");
            Field(h => h.Name, nullable: true).Description("The name of the product.");
            Field(h => h.Description, nullable: true).Description("The description of the product.");
        }
    }
}
