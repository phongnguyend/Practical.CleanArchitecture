using ClassifiedAds.Application;
using ClassifiedAds.Services.Identity.Entities;
using ClassifiedAds.Services.Identity.Repositories;

namespace ClassifiedAds.Services.Identity.Commands.Roles
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
