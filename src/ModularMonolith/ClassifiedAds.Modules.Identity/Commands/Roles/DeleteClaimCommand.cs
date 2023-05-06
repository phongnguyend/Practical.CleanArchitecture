using ClassifiedAds.Application;
using ClassifiedAds.Modules.Identity.Entities;
using ClassifiedAds.Modules.Identity.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Modules.Identity.Commands.Roles;

public class DeleteClaimCommand : ICommand
{
    public Role Role { get; set; }
    public RoleClaim Claim { get; set; }
}

public class DeleteClaimCommandHandler : ICommandHandler<DeleteClaimCommand>
{
    private readonly IRoleRepository _roleRepository;

    public DeleteClaimCommandHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task HandleAsync(DeleteClaimCommand command, CancellationToken cancellationToken = default)
    {
        command.Role.Claims.Remove(command.Claim);
        await _roleRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
