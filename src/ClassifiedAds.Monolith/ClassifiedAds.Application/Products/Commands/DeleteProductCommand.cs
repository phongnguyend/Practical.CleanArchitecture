using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Repositories;
using System;

namespace ClassifiedAds.Application.Products.Commands
{
    public class DeleteProductCommand : ICommand
    {
        public Product Product { get; set; }
    }

    internal class DeleteProductCommandHandler : ICommandHandler<DeleteProductCommand>
    {
        private readonly IRepository<Product, Guid> _productRepository;

        public DeleteProductCommandHandler(IRepository<Product, Guid> productRepository)
        {
            _productRepository = productRepository;
        }

        public void Handle(DeleteProductCommand command)
        {
            _productRepository.Delete(command.Product);
            _productRepository.UnitOfWork.SaveChanges();
        }
    }
}
