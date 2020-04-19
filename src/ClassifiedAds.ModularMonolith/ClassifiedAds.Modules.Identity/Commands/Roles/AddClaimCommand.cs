using ClassifiedAds.Modules.Identity.Entities;
using ClassifiedAds.Modules.Identity.Repositories;

namespace ClassifiedAds.Application.Roles.Commands
{
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

        public void Handle(AddClaimCommand command)
        {
            command.Role.Claims.Add(command.Claim);
            _roleRepository.UnitOfWork.SaveChanges();
        }
    }
}
