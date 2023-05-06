using ClassifiedAds.Application;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Modules.Product.Commands;

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

    public async Task HandleAsync(DeleteProductCommand command, CancellationToken cancellationToken = default)
    {
        await _productService.DeleteAsync(command.Product);
    }
}
