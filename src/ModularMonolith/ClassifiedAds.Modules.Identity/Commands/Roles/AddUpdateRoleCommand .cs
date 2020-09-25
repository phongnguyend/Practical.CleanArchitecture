using ClassifiedAds.Application;
using ClassifiedAds.Modules.Identity.Entities;
using ClassifiedAds.Modules.Identity.Repositories;

namespace ClassifiedAds.Modules.Identity.Commands.Roles
{
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

        public void Handle(AddUpdateRoleCommand command)
        {
            _roleRepository.AddOrUpdate(command.Role);
            _roleRepository.UnitOfWork.SaveChanges();
        }
    }
}
