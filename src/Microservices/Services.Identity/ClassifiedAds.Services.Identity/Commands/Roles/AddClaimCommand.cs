using ClassifiedAds.Application;
using ClassifiedAds.Services.Identity.Entities;
using ClassifiedAds.Services.Identity.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Identity.Commands.Roles;

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
