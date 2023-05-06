using ClassifiedAds.Application;
using ClassifiedAds.Services.Identity.Entities;
using ClassifiedAds.Services.Identity.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Identity.Commands.Users;

public class DeleteRoleCommand : ICommand
{
    public User User { get; set; }
    public UserRole Role { get; set; }
}

public class DeleteRoleCommandHandler : ICommandHandler<DeleteRoleCommand>
{
    private readonly IUserRepository _userRepository;

    public DeleteRoleCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task HandleAsync(DeleteRoleCommand command, CancellationToken cancellationToken = default)
    {
        command.User.UserRoles.Remove(command.Role);
        await _userRepository.AddOrUpdateAsync(command.User);
        await _userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
