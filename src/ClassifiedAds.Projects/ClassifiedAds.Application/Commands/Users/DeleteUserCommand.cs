using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Services;

namespace ClassifiedAds.Application.Commands.Users
{
    public class DeleteUserCommand : ICommand
    {
        public User User { get; set; }
    }

    internal class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand>
    {
        private readonly IUserService _userService;

        public DeleteUserCommandHandler(IUserService productService)
        {
            _userService = productService;
        }

        public void Handle(DeleteUserCommand command)
        {
            _userService.Delete(command.User);
        }
    }
}
