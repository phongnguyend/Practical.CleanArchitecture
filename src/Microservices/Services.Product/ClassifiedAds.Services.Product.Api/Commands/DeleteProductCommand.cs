using ClassifiedAds.Application;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Product.Commands;

public class DeleteProductCommand : IRequest
{
    public Entities.Product Product { get; set; }
}

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly ICrudService<Entities.Product> _productService;

    public DeleteProductCommandHandler(ICrudService<Entities.Product> productService)
    {
        _productService = productService;
    }

    public async Task Handle(DeleteProductCommand command, CancellationToken cancellationToken = default)
    {
        await _productService.DeleteAsync(command.Product);
    }
}
