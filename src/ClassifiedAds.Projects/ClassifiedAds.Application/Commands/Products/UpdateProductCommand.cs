using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Services;

namespace ClassifiedAds.Application.Commands.Products
{
    public class UpdateProductCommand : ICommand
    {
        public Product Product { get; set; }
    }

    internal class UpdateProductCommandHandler : ICommandHandler<UpdateProductCommand>
    {
        private readonly IProductService _productService;

        public UpdateProductCommandHandler(IProductService productService)
        {
            _productService = productService;
        }

        public void Handle(UpdateProductCommand command)
        {
            _productService.Update(command.Product);
        }
    }
}
