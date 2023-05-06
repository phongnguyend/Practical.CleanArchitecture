using ClassifiedAds.Application;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Modules.Product.Commands;

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

    public async Task HandleAsync(AddUpdateProductCommand command, CancellationToken cancellationToken = default)
    {
        await _productService.AddOrUpdateAsync(command.Product);
    }
}
