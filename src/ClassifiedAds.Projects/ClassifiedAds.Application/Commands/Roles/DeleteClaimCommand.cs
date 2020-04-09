using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Repositories;

namespace ClassifiedAds.Application.Commands.Roles
{
    public class DeleteClaimCommand : ICommand
    {
        public Role Role { get; set; }
        public RoleClaim Claim { get; set; }
    }

    internal class DeleteClaimCommandHandler : ICommandHandler<DeleteClaimCommand>
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
