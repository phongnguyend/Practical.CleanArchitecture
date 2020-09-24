using ClassifiedAds.Services.Identity.Entities;
using ClassifiedAds.Services.Identity.Repositories;

namespace ClassifiedAds.Application.Roles.Commands
{
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

        public void Handle(DeleteClaimCommand command)
        {
            command.Role.Claims.Remove(command.Claim);
            _roleRepository.UnitOfWork.SaveChanges();
        }
    }
}
