using ClassifiedAds.Modules.Identity.Entities;
using ClassifiedAds.Modules.Identity.Repositories;

namespace ClassifiedAds.Application.Users.Commands
{
    public class AddRoleCommand : ICommand
    {
        public User User { get; set; }
        public UserRole Role { get; set; }
    }

    public class AddRoleCommandHandler : ICommandHandler<AddRoleCommand>
    {
        private readonly IUserRepository _userRepository;

        public AddRoleCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public void Handle(AddRoleCommand command)
        {
            command.User.UserRoles.Add(command.Role);
            _userRepository.AddOrUpdate(command.User);
            _userRepository.UnitOfWork.SaveChanges();
        }
    }
}
