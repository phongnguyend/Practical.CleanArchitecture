using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Services;

namespace ClassifiedAds.Application.Commands.Products
{
    public class AddUpdateProductCommand : ICommand
    {
        public Product Product { get; set; }
    }

    internal class AddUpdateProductCommandHandler : ICommandHandler<AddUpdateProductCommand>
    {
        private readonly IProductService _productService;

        public AddUpdateProductCommandHandler(IProductService productService)
        {
            _productService = productService;
        }

        public void Handle(AddUpdateProductCommand command)
        {
            _productService.AddOrUpdate(command.Product);
        }
    }
}
