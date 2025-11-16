using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Application.Products.Commands;

public class AddUpdateProductCommand : ICommand
{
    public Product Product { get; set; }
}

internal class AddUpdateProductCommandHandler : ICommandHandler<AddUpdateProductCommand>
{
    private readonly ICrudService<Product> _productService;
    private readonly IUnitOfWork _unitOfWork;

    public AddUpdateProductCommandHandler(ICrudService<Product> productService, IUnitOfWork unitOfWork)
    {
        _productService = productService;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(AddUpdateProductCommand command, CancellationToken cancellationToken = default)
    {
        using (await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted, cancellationToken))
        {
            await _productService.AddOrUpdateAsync(command.Product, cancellationToken);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
    }
}
