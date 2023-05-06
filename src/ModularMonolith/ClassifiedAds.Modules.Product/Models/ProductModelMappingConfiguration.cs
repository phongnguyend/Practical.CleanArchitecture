using System.Collections.Generic;
using System.Linq;

namespace ClassifiedAds.Modules.Product.Models;

public static class ProductModelMappingConfiguration
{
    public static IEnumerable<ProductModel> ToModels(this IEnumerable<Entities.Product> entities)
    {
        return entities.Select(x => x.ToModel());
    }

    public static ProductModel ToModel(this Entities.Product entity)
    {
        if (entity == null)
        {
            return null;
        }

        return new ProductModel
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            Description = entity.Description,
        };
    }

    public static Entities.Product ToEntity(this ProductModel model)
    {
        return new Entities.Product
        {
            Id = model.Id,
            Code = model.Code,
            Name = model.Name,
            Description = model.Description,
        };
    }
}
