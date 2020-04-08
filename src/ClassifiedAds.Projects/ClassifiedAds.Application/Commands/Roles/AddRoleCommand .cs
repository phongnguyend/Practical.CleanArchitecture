using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Repositories;

namespace ClassifiedAds.Application.Commands.Roles
{
    public class AddRoleCommand : ICommand
    {
        public Role Role { get; set; }
    }

    internal class AddRoleCommandHandler : ICommandHandler<AddRoleCommand>
    {
        private readonly IRoleRepository _roleRepository;

        public AddRoleCommandHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public void Handle(AddRoleCommand command)
        {
            _roleRepository.Add(command.Role);
            _roleRepository.UnitOfWork.SaveChanges();
        }
    }
}
