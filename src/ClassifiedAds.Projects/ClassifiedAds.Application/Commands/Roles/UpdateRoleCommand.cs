using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Repositories;

namespace ClassifiedAds.Application.Commands.Roles
{
    public class UpdateRoleCommand : ICommand
    {
        public Role Role { get; set; }
    }

    internal class UpdateRoleCommandHandler : ICommandHandler<UpdateRoleCommand>
    {
        private readonly IRoleRepository _roleRepository;

        public UpdateRoleCommandHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public void Handle(UpdateRoleCommand command)
        {
            _roleRepository.UnitOfWork.SaveChanges();
        }
    }
}
