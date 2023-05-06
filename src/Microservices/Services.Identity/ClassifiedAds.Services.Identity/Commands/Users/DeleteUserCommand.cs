using ClassifiedAds.Application;
using ClassifiedAds.Services.Identity.Entities;
using ClassifiedAds.Services.Identity.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Identity.Commands.Users;

public class DeleteUserCommand : ICommand
{
    public User User { get; set; }
}

public class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand>
{
    private readonly IUserRepository _userRepository;

    public DeleteUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task HandleAsync(DeleteUserCommand command, CancellationToken cancellationToken = default)
    {
        _userRepository.Delete(command.User);
        await _userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
