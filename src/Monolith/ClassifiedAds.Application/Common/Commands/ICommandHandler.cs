using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Application;

public interface ICommandHandler<TCommand>
    where TCommand : ICommand
{
    Task HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}
