using AutoMapper;
using ClassifiedAds.Domain.Entities;

namespace ClassifiedAds.WebAPI.Models.Products
{
    public class ProductModelMappingConfiguration : Profile
    {
        public ProductModelMappingConfiguration()
        {
            CreateMap<Product, ProductModel>().ReverseMap();
        }
    }
}
