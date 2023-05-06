using ClassifiedAds.Application;
using ClassifiedAds.Modules.Identity.Entities;
using ClassifiedAds.Modules.Identity.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Modules.Identity.Commands.Roles;

public class AddUpdateRoleCommand : ICommand
{
    public Role Role { get; set; }
}

public class AddUpdateRoleCommandHandler : ICommandHandler<AddUpdateRoleCommand>
{
    private readonly IRoleRepository _roleRepository;

    public AddUpdateRoleCommandHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task HandleAsync(AddUpdateRoleCommand command, CancellationToken cancellationToken = default)
    {
        await _roleRepository.AddOrUpdateAsync(command.Role);
        await _roleRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
