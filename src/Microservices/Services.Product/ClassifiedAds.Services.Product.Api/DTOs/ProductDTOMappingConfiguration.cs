using AutoMapper;

namespace ClassifiedAds.Services.Product.DTOs.Products
{
    public class ProductDTOMappingConfiguration : Profile
    {
        public ProductDTOMappingConfiguration()
        {
            CreateMap<Entities.Product, ProductDTO>().ReverseMap();
        }
    }
}
