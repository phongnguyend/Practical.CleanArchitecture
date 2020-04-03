using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Services;

namespace ClassifiedAds.Application.Commands.Products
{
    public class AddProductCommand : ICommand
    {
        public Product Product { get; set; }
    }

    internal class AddProductCommandHandler : ICommandHandler<AddProductCommand>
    {
        private readonly IProductService _productService;

        public AddProductCommandHandler(IProductService productService)
        {
            _productService = productService;
        }

        public void Handle(AddProductCommand command)
        {
            _productService.Add(command.Product);
        }
    }
}
