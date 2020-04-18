using ClassifiedAds.Domain.Entities;

namespace ClassifiedAds.Application.Products.Commands
{
    public class AddUpdateProductCommand : ICommand
    {
        public Product Product { get; set; }
    }

    internal class AddUpdateProductCommandHandler : ICommandHandler<AddUpdateProductCommand>
    {
        private readonly ICrudService<Product> _productService;

        public AddUpdateProductCommandHandler(ICrudService<Product> productService)
        {
            _productService = productService;
        }

        public void Handle(AddUpdateProductCommand command)
        {
            _productService.AddOrUpdate(command.Product);
        }
    }
}
