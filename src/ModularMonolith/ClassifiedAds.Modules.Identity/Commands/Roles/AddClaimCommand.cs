using ClassifiedAds.Application;
using ClassifiedAds.Modules.Identity.Entities;
using ClassifiedAds.Modules.Identity.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Modules.Identity.Commands.Roles;

public class AddClaimCommand : ICommand
{
    public Role Role { get; set; }
    public RoleClaim Claim { get; set; }
}

public class AddClaimCommandHandler : ICommandHandler<AddClaimCommand>
{
    private readonly IRoleRepository _roleRepository;

    public AddClaimCommandHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task HandleAsync(AddClaimCommand command, CancellationToken cancellationToken = default)
    {
        command.Role.Claims.Add(command.Claim);
        await _roleRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
