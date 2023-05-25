using ClassifiedAds.Application;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Product.Commands;

public class AddUpdateProductCommand : IRequest
{
    public Entities.Product Product { get; set; }
}

public class AddUpdateProductCommandHandler : IRequestHandler<AddUpdateProductCommand>
{
    private readonly ICrudService<Entities.Product> _productService;

    public AddUpdateProductCommandHandler(ICrudService<Entities.Product> productService)
    {
        _productService = productService;
    }

    public async Task Handle(AddUpdateProductCommand command, CancellationToken cancellationToken = default)
    {
        await _productService.AddOrUpdateAsync(command.Product);
    }
}
