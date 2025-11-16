using ClassifiedAds.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Application.Products.Commands;

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

    public async Task HandleAsync(DeleteProductCommand command, CancellationToken cancellationToken = default)
    {
        await _productService.DeleteAsync(command.Product, cancellationToken);
    }
}
