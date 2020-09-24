using AutoMapper;

namespace ClassifiedAds.Modules.Product.DTOs
{
    public class ProductDTOMappingConfiguration : Profile
    {
        public ProductDTOMappingConfiguration()
        {
            CreateMap<Entities.Product, ProductDTO>().ReverseMap();
        }
    }
}
