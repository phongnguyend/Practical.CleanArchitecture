using ClassifiedAds.Application;
using ClassifiedAds.Modules.Product.Services;

namespace ClassifiedAds.Modules.Product.Commands
{
    public class AddUpdateProductCommand : ICommand
    {
        public Entities.Product Product { get; set; }
    }

    public class AddUpdateProductCommandHandler : ICommandHandler<AddUpdateProductCommand>
    {
        private readonly ICrudService<Entities.Product> _productService;

        public AddUpdateProductCommandHandler(ICrudService<Entities.Product> productService)
        {
            _productService = productService;
        }

        public void Handle(AddUpdateProductCommand command)
        {
            _productService.AddOrUpdate(command.Product);
        }
    }
}
