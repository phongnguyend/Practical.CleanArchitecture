
using System.Collections.Generic;
using System.Linq;

namespace ClassifiedAds.Services.Product.DTOs
{
    public static class ProductDTOMappingConfiguration
    {
        public static IEnumerable<ProductDTO> ToDTOs(this IEnumerable<Entities.Product> entities)
        {
            return entities.Select(x => x.ToDTO());
        }

        public static ProductDTO ToDTO(this Entities.Product entity)
        {
            return new ProductDTO
            {
                Id = entity.Id,
                Code = entity.Code,
                Name = entity.Name,
                Description = entity.Description,
            };
        }

        public static Entities.Product ToEntity(this ProductDTO dto)
        {
            return new Entities.Product
            {
                Id = dto.Id,
                Code = dto.Code,
                Name = dto.Name,
                Description = dto.Description,
            };
        }
    }
}
