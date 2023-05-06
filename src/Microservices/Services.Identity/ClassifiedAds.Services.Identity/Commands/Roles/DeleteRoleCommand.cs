using ClassifiedAds.Application;
using ClassifiedAds.Services.Identity.Entities;
using ClassifiedAds.Services.Identity.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Identity.Commands.Roles;

public class DeleteRoleCommand : ICommand
{
    public Role Role { get; set; }
}

public class DeleteRoleCommandHandler : ICommandHandler<DeleteRoleCommand>
{
    private readonly IRoleRepository _roleRepository;

    public DeleteRoleCommandHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task HandleAsync(DeleteRoleCommand command, CancellationToken cancellationToken = default)
    {
        _roleRepository.Delete(command.Role);
        await _roleRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
