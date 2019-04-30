using ClassifiedAds.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClassifiedAds.DomainServices
{
    public interface IProductService
    {
        IEnumerable<Product> GetProducts();

        Product GetById(Guid Id);

        Product Create(Product product);

        void Delete(Product product);
    }
}
