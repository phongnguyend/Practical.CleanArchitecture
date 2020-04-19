using ClassifiedAds.Modules.Identity.Entities;
using ClassifiedAds.Modules.Identity.Repositories;

namespace ClassifiedAds.Application.Users.Commands
{
    public class DeleteUserCommand : ICommand
    {
        public User User { get; set; }
    }

    public class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand>
    {
        private readonly IUserRepository _userRepository;

        public DeleteUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public void Handle(DeleteUserCommand command)
        {
            _userRepository.Delete(command.User);
            _userRepository.UnitOfWork.SaveChanges();
        }
    }
}
