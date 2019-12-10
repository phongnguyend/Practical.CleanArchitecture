using System;
using System.Collections.Generic;
using System.Linq;
using ClassifiedAds.DomainServices.Entities;
using ClassifiedAds.DomainServices.Identity;
using ClassifiedAds.DomainServices.Repositories;

namespace ClassifiedAds.DomainServices
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Product> _productRepository;

        public ProductService(IUnitOfWork unitOfWork, IRepository<Product> productRepository, ICurrentUser currentUser)
        {
            _unitOfWork = unitOfWork;
            _productRepository = productRepository;
        }

        public IEnumerable<Product> GetProducts()
        {
            return _productRepository.GetAll();
        }

        public Product Create(Product product)
        {
            _productRepository.Add(product);
            _unitOfWork.SaveChanges();
            return product;
        }

        public Product Update(Product product)
        {
            _unitOfWork.SaveChanges();
            return product;
        }

        public void Delete(Product product)
        {
            _productRepository.Delete(product);
            _unitOfWork.SaveChanges();
        }

        public Product GetById(Guid Id)
        {
            return _productRepository.GetAll().FirstOrDefault(x => x.Id == Id);
        }
    }
}
