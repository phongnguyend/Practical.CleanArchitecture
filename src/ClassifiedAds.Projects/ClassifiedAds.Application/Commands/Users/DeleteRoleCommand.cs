using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Services;

namespace ClassifiedAds.Application.Commands.Users
{
    public class DeleteRoleCommand : ICommand
    {
        public User User { get; set; }
        public UserRole Role { get; set; }
    }

    internal class DeleteRoleCommandHandler : ICommandHandler<DeleteRoleCommand>
    {
        private readonly IUserService _userService;

        public DeleteRoleCommandHandler(IUserService productService)
        {
            _userService = productService;
        }

        public void Handle(DeleteRoleCommand command)
        {
            command.User.UserRoles.Remove(command.Role);
            _userService.AddOrUpdate(command.User);
        }
    }
}
