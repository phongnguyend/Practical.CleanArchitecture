using ClassifiedAds.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

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

        public async Task HandleAsync(AddUpdateProductCommand command, CancellationToken cancellationToken = default)
        {
            await _productService.AddOrUpdateAsync(command.Product);
        }
    }
}
