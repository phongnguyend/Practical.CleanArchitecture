using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Repositories;

namespace ClassifiedAds.Application.Commands.Roles
{
    public class AddClaimCommand : ICommand
    {
        public Role Role { get; set; }
        public RoleClaim Claim { get; set; }
    }

    internal class AddClaimCommandHandler : ICommandHandler<AddClaimCommand>
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
