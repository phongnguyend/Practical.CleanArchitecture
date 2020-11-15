using ClassifiedAds.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace ClassifiedAds.WebAPI.Models.Products
{
    public static class ProductModelMappingConfiguration
    {
        public static IEnumerable<ProductModel> ToDTOs(this IEnumerable<Product> entities)
        {
            return entities.Select(x => x.ToDTO());
        }

        public static ProductModel ToDTO(this Product entity)
        {
            return new ProductModel
            {
                Id = entity.Id,
                Code = entity.Code,
                Name = entity.Name,
                Description = entity.Description,
            };
        }

        public static Product ToEntity(this ProductModel dto)
        {
            return new Product
            {
                Id = dto.Id,
                Code = dto.Code,
                Name = dto.Name,
                Description = dto.Description,
            };
        }
    }
}
