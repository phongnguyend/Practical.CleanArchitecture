using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Application.Users.Commands;

public class AddClaimCommand : ICommand
{
    public User User { get; set; }
    public UserClaim Claim { get; set; }
}

internal class AddClaimCommandHandler : ICommandHandler<AddClaimCommand>
{
    private readonly IUserRepository _userRepository;

    public AddClaimCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task HandleAsync(AddClaimCommand command, CancellationToken cancellationToken = default)
    {
        command.User.Claims.Add(command.Claim);
        await _userRepository.AddOrUpdateAsync(command.User, cancellationToken);
        await _userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
