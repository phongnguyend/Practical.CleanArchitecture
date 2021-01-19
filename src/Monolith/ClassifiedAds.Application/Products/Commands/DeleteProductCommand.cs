using ClassifiedAds.Domain.Entities;
using System.Threading.Tasks;

namespace ClassifiedAds.Application.Products.Commands
{
    public class DeleteProductCommand : ICommand
    {
        public Product Product { get; set; }
    }

    internal class DeleteProductCommandHandler : ICommandHandler<DeleteProductCommand>
    {
        private readonly ICrudService<Product> _productService;

        public DeleteProductCommandHandler(ICrudService<Product> productService)
        {
            _productService = productService;
        }

        public async Task HandleAsync(DeleteProductCommand command)
        {
            await _productService.DeleteAsync(command.Product);
        }
    }
}
