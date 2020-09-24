using ClassifiedAds.Application;

namespace ClassifiedAds.Services.Product.Commands
{
    public class DeleteProductCommand : ICommand
    {
        public Entities.Product Product { get; set; }
    }

    public class DeleteProductCommandHandler : ICommandHandler<DeleteProductCommand>
    {
        private readonly ICrudService<Entities.Product> _productService;

        public DeleteProductCommandHandler(ICrudService<Entities.Product> productService)
        {
            _productService = productService;
        }

        public void Handle(DeleteProductCommand command)
        {
            _productService.Delete(command.Product);
        }
    }
}
