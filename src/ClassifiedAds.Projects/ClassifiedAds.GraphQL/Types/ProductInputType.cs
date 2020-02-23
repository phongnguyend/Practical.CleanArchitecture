using ClassifiedAds.Domain.Entities;
using GraphQL.Types;

namespace ClassifiedAds.GraphQL.Types
{
    public class ProductInputType : InputObjectGraphType<Product>
    {
        public ProductInputType()
        {
            Name = "ProductInput";
            Field(x => x.Code);
            Field(x => x.Name);
            Field(x => x.Description, nullable: true);
        }
    }
}
