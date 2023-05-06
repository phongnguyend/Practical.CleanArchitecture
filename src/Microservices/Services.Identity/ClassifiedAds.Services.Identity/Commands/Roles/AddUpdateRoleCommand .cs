using ClassifiedAds.Application;
using ClassifiedAds.Services.Identity.Entities;
using ClassifiedAds.Services.Identity.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Identity.Commands.Roles;

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
