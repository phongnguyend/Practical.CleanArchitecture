using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Application.Users.Commands;

public class DeleteClaimCommand : ICommand
{
    public User User { get; set; }
    public UserClaim Claim { get; set; }
}

internal class DeleteClaimCommandHandler : ICommandHandler<DeleteClaimCommand>
{
    private readonly IUserRepository _userRepository;

    public DeleteClaimCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task HandleAsync(DeleteClaimCommand command, CancellationToken cancellationToken = default)
    {
        command.User.Claims.Remove(command.Claim);
        await _userRepository.AddOrUpdateAsync(command.User, cancellationToken);
        await _userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
