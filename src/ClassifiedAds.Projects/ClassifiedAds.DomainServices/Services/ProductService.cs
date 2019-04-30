using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClassifiedAds.Domain.Entities;

namespace ClassifiedAds.DomainServices
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _productRepository;

        public ProductService(IRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public IEnumerable<Product> GetProducts()
        {
            return _productRepository.GetAll();
        }

        public Product Create(Product product)
        {
            _productRepository.Add(product);
            return product;
        }

        public void Delete(Product product)
        {
            _productRepository.Delete(product);
        }

        public Product GetById(Guid Id)
        {
            return _productRepository.GetAll().FirstOrDefault(x => x.Id == Id);
        }
    }
}
