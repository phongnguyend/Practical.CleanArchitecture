using ClassifiedAds.Services.Identity.Entities;
using ClassifiedAds.Services.Identity.Repositories;

namespace ClassifiedAds.Application.Roles.Commands
{
    public class DeleteRoleCommand : ICommand
    {
        public Role Role { get; set; }
    }

    public class DeleteRoleCommandHandler : ICommandHandler<DeleteRoleCommand>
    {
        private readonly IRoleRepository _roleRepository;

        public DeleteRoleCommandHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public void Handle(DeleteRoleCommand command)
        {
            _roleRepository.Delete(command.Role);
            _roleRepository.UnitOfWork.SaveChanges();
        }
    }
}
