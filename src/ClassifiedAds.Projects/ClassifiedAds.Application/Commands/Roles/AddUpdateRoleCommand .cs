using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Repositories;

namespace ClassifiedAds.Application.Commands.Roles
{
    public class AddUpdateRoleCommand : ICommand
    {
        public Role Role { get; set; }
    }

    internal class AddUpdateRoleCommandHandler : ICommandHandler<AddUpdateRoleCommand>
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
