using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Repositories;
using System;

namespace ClassifiedAds.Application.Products.Commands
{
    public class AddUpdateProductCommand : ICommand
    {
        public Product Product { get; set; }
    }

    internal class AddUpdateProductCommandHandler : ICommandHandler<AddUpdateProductCommand>
    {
        private readonly IRepository<Product, Guid> _productRepository;

        public AddUpdateProductCommandHandler(IRepository<Product, Guid> productRepository)
        {
            _productRepository = productRepository;
        }

        public void Handle(AddUpdateProductCommand command)
        {
            _productRepository.AddOrUpdate(command.Product);
            _productRepository.UnitOfWork.SaveChanges();
        }
    }
}
