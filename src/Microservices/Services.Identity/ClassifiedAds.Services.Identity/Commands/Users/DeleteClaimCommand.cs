using ClassifiedAds.Application;
using ClassifiedAds.Services.Identity.Entities;
using ClassifiedAds.Services.Identity.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Identity.Commands.Users;

public class DeleteClaimCommand : ICommand
{
    public User User { get; set; }
    public UserClaim Claim { get; set; }
}

public class DeleteClaimCommandHandler : ICommandHandler<DeleteClaimCommand>
{
    private readonly IUserRepository _userRepository;

    public DeleteClaimCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task HandleAsync(DeleteClaimCommand command, CancellationToken cancellationToken = default)
    {
        command.User.Claims.Remove(command.Claim);
        await _userRepository.AddOrUpdateAsync(command.User);
        await _userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
